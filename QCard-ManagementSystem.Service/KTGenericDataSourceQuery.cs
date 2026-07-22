using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace QCard.ManagementSystem.Service
{
    public class KTGenericDataSourceQuery
    {
        protected const string TABLE_ALIAS = "gdb";
        private readonly Dictionary<string, string> _operators = new Dictionary<string, string>();
        protected readonly List<SqlParameter> _paraCols = new List<SqlParameter>();
        protected IDbConnection Connection { get; set; }
        protected KTDataTableRequest DataTableRequest { get; set; }
        public string FetchQuery = "SELECT TOP {0} * FROM (SELECT *, ROW_NUMBER() OVER (ORDER BY {4}) as row_number FROM (SELECT * FROM ({1}) AS {2}) AS {2} WHERE ({3})) AS {2} WHERE {2}.row_number >= {5}";
        public KTGenericDataSourceQuery(IDbConnection connection, KTDataTableRequest dataTableRequest)
        {
            Connection = connection;
            DataTableRequest = dataTableRequest;

            _operators.Add("contains", "{0} LIKE '%' + {1} + '%'");
            _operators.Add("does_not_contain", "{0} NOT LIKE '%' + {1} + '%'");
            _operators.Add("ends_with", "{0} LIKE '%' + {1}");
            _operators.Add("starts_with", "{0} LIKE {1} + '%'");
            _operators.Add("is_null", "{0} IS NULL");
            _operators.Add("is_not_null", "{0} IS NOT NULL");
            _operators.Add("is_not_equal_to", "{0} <> {1}");
            _operators.Add("is_not_empty", "{0} <> ''");
            _operators.Add("is_less_than_or_equal_to", "{0} <= {1}");
            _operators.Add("is_less_than", "{0} < {1}");
            _operators.Add("is_greater_than_or_equal_to", "{0} >= {1}");
            _operators.Add("is_greater_than", "{0} > {1}");
            _operators.Add("is_equal_to", "{0} = {1}");
            _operators.Add("is_contained_in", "{0} IN({1})");
        }

        public KTDataTableResponse GetData<T>(string storedFunction, Dictionary<string, object> parameters, Dictionary<string, object> conditionals = null)
        {
            var whereClause = string.Empty;

            if (parameters != null)
            {
                foreach (var key in parameters.Keys)
                {
                    _paraCols.Add(new SqlParameter(key, parameters[key]));
                }
            }

            var parameterString = parameters != null ? string.Join(", ", _paraCols.Select(x => "@" + x.ParameterName)) : string.Empty;

            if (conditionals != null)
            {
                foreach (var key in conditionals.Keys)
                {
                    _paraCols.Add(new SqlParameter(key, conditionals[key]));
                }

                whereClause = "WHERE " + string.Join(" AND ", conditionals.Select(x => x.Key + " = @" + x.Key));
            }

            return GetData<T>(string.Format("SELECT * FROM {0}({1}) {2}", storedFunction, parameterString, whereClause));
        }
        public KTDataTableResponse GetData<T>(Dictionary<string, object> parameters)
        {
            var whereClause = string.Empty;

            if (parameters != null)
            {
                foreach (var key in parameters.Keys)
                {
                    _paraCols.Add(new SqlParameter(key, parameters[key]));
                }

                whereClause = "WHERE " + string.Join(" AND ", parameters.Select(x => x.Key + " = @" + x.Key));
            }
            var type = typeof(T);
            var attribute = type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;

            return GetData<T>(string.Format("SELECT * FROM [{0}] {1}", attribute != null ? attribute.Name : type.Name, whereClause));
        }
        public KTDataTableResponse GetData<T>()
        {
            var whereClause = string.Empty;

            var type = typeof(T);
            var attribute = type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;

            return GetData<T>(string.Format("SELECT * FROM [{0}] {1}", attribute != null ? attribute.Name : type.Name, whereClause));
        }
        public KTDataTableResponse GetData<T>(string query)
        {
            int displayLength = DataTableRequest.Pagination.Perpage == 0 ? int.MaxValue : DataTableRequest.Pagination.Perpage;
            int displayStart = (DataTableRequest.Pagination.Page - 1) * displayLength + 1;

            //string filterQuery = BuildFilter(DataTableRequest.Filters.ToList(), "AND");
            string filterQuery = DataTableRequest.Filters != null ? BuildFilter(DataTableRequest.Filters.ToList(), "AND") : "1=1";

            string sortQuery = BuildSort<T>();
            string sqlQuery =
                string.Format(FetchQuery, displayLength, query, TABLE_ALIAS, filterQuery, sortQuery, displayStart);
            int totalCount = GetTotalCount(query, filterQuery);
            List<T> result = null;

            DynamicParameters parameters = new DynamicParameters();
            _paraCols.ForEach(x => parameters.Add(x.ParameterName, x.Value));

            result = Connection.Query<T>(
                    sqlQuery,
                    param: parameters, commandTimeout: 0
                ).ToList();

            _paraCols.Clear();

            return new KTDataTableResponse
            {
                meta = new KTDataTableResponseMeta
                {
                    field = DataTableRequest.Sort.Field,
                    pages = (totalCount + DataTableRequest.Pagination.Perpage - 1) / DataTableRequest.Pagination.Perpage,
                    page = DataTableRequest.Pagination.Page,
                    perpage = DataTableRequest.Pagination.Perpage,
                    sort = DataTableRequest.Sort.Sort,
                    total = totalCount,
                },
                data = result.Select(x => x as object).ToArray()
            };
        }
        protected virtual int GetTotalCount(string query, string filterQuery)
        {
            var sqlString = string.Format("SELECT COUNT(*) FROM ({0}) AS {1} WHERE ({2})", query, TABLE_ALIAS, string.IsNullOrEmpty(filterQuery) ? "1=1" : filterQuery);

            DynamicParameters parameters = new DynamicParameters();
            _paraCols.ForEach(x => parameters.Add(x.ParameterName, x.Value));

            return Connection.Query<int>(sqlString, parameters).FirstOrDefault();
        }


        protected virtual string BuildFilter(List<KTDataTableFilter> searchables, string operand)
        {
            var qb = new List<string>();

            foreach (var filterDescriptor in searchables)
            {
                if(filterDescriptor.Filters?.Count() > 0)
                {
                    qb.Add(BuildFilter(filterDescriptor.Filters.ToList(), filterDescriptor.Logic));
                } else
                {
                    var filter = filterDescriptor.Field;
                    var index = _paraCols.Count;
                    var paramName = "@" + filterDescriptor.Field + index;
                    _paraCols.Add(new SqlParameter(paramName, filterDescriptor.Value));

                    qb.Add(string.Format(_operators[filterDescriptor.Operator], filter, paramName));
                }
            }

            var query = "(" + (qb.Count > 0 ? string.Join(string.Format(" {0} ", operand), qb) : "1 = 1") + ")";

            return query;
        }

        protected virtual string BuildSort<T>()
        {
            var sortList = new List<string>();

            if (DataTableRequest.Sort != null)
            {
                sortList.Add(string.Format("{0} {1}", DataTableRequest.Sort.Field, DataTableRequest.Sort.Sort.ToUpper() == "ASC" ? "ASC" : "DESC"));
            }
            else
            {
                //var type = typeof(T);
                //var propertyName = typeof(T) != typeof(object) ? type.GetProperties().First().Name : "ID";

                //sortList.Add(propertyName + " ASC");
                sortList.Add("ID ASC");

                DataTableRequest.Sort = new KTDataTableSort
                {
                    Field = "ID",
                    Sort = "asc"
                };
            }

            return string.Join(", ", sortList);
        }
    }
}
