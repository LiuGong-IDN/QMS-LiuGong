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
using Kendo.Mvc;
using Kendo.Mvc.UI;

namespace QCard.ManagementSystem.Domain
{
    public class GenericDataSourceQuery
    {
        protected const string TABLE_ALIAS = "gdb";
        private readonly Dictionary<FilterOperator, string> _operators = new Dictionary<FilterOperator, string>();
        protected readonly List<SqlParameter> _paraCols = new List<SqlParameter>();
        protected IDbConnection Connection { get; set; }
        protected DataSourceRequest DataTableRequest { get; set; }
        public string FetchQuery = "SELECT TOP {0} * FROM (SELECT *, ROW_NUMBER() OVER (ORDER BY {4}) as row_number FROM (SELECT * FROM ({1}) AS {2}) AS {2} WHERE ({3})) AS {2} WHERE {2}.row_number >= {5} order by row_number";
        public GenericDataSourceQuery(IDbConnection connection, DataSourceRequest dataTableRequest)
        {
            Connection = connection;
            DataTableRequest = dataTableRequest;

            _operators.Add(FilterOperator.Contains, "{0} LIKE '%' + {1} + '%'");
            _operators.Add(FilterOperator.DoesNotContain, "{0} NOT LIKE '%' + {1} + '%'");
            _operators.Add(FilterOperator.EndsWith, "{0} LIKE '%' + {1}");
            _operators.Add(FilterOperator.StartsWith, "{0} LIKE {1} + '%'");
            _operators.Add(FilterOperator.IsNull, "{0} IS NULL");
            _operators.Add(FilterOperator.IsNotNull, "{0} IS NOT NULL");
            _operators.Add(FilterOperator.IsNotEqualTo, "{0} <> {1}");
            _operators.Add(FilterOperator.IsNotEmpty, "{0} <> ''");
            _operators.Add(FilterOperator.IsLessThanOrEqualTo, "{0} <= {1}");
            _operators.Add(FilterOperator.IsLessThan, "{0} < {1}");
            _operators.Add(FilterOperator.IsGreaterThanOrEqualTo, "{0} >= {1}");
            _operators.Add(FilterOperator.IsGreaterThan, "{0} > {1}");
            _operators.Add(FilterOperator.IsEqualTo, "{0} = {1}");
            _operators.Add(FilterOperator.IsContainedIn, "{0} IN({1})");
        }
        public DataSourceResult GetData<T>(string storedFunction, Dictionary<string, object> parameters, Dictionary<string, object> conditionals = null)
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
        public DataSourceResult GetData<T>(Dictionary<string, object> parameters)
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
        public DataSourceResult GetData<T>()
        {
            var whereClause = string.Empty;

            var type = typeof(T);
            var attribute = type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;

            return GetData<T>(string.Format("SELECT * FROM [{0}] {1}", attribute != null ? attribute.Name : type.Name, whereClause));
        }
        public DataSourceResult GetData<T>(string query)
        {
            int displayLength = DataTableRequest.PageSize == 0 ? int.MaxValue : DataTableRequest.PageSize;
            int displayStart = (DataTableRequest.Page - 1) * displayLength + 1;

            //string filterQuery = BuildFilter(DataTableRequest.Filters.ToList(), "AND");
            string filterQuery = DataTableRequest.Filters != null ? BuildFilter(DataTableRequest.Filters.ToList(), "AND") : "1=1";
            string sortQuery = BuildSort<T>();
            string sqlQuery =
                string.Format(FetchQuery, displayLength, query, TABLE_ALIAS, filterQuery, sortQuery, displayStart);
            int totalCount = GetTotalCount(query, filterQuery);
            IEnumerable data = null;

            DynamicParameters parameters = new DynamicParameters();
            _paraCols.ForEach(x => parameters.Add(x.ParameterName, x.Value == null ? "" : x.Value.ToString()));

            data = Connection.Query<T>(
                    sqlQuery,
                    param: parameters, commandTimeout: int.MaxValue
                ).ToList();

            _paraCols.Clear();

            return new DataSourceResult { Data = data, Total = totalCount };
        }
        protected virtual int GetTotalCount(string query, string filterQuery)
        {
            var sqlString = string.Format("SELECT COUNT(*) FROM ({0}) AS {1} WHERE ({2})", query, TABLE_ALIAS, string.IsNullOrEmpty(filterQuery) ? "1=1" : filterQuery);

            DynamicParameters parameters = new DynamicParameters();
            _paraCols.ForEach(x => parameters.Add(x.ParameterName, x.Value == null ? "" : x.Value.ToString()));

            return Connection.Query<int>(sqlString, parameters,commandTimeout:int.MaxValue).FirstOrDefault();
        }
        protected virtual string BuildFilter(List<IFilterDescriptor> searchables, string operand)
        {
            var qb = new List<string>();

            foreach (IFilterDescriptor filterDescriptor in searchables)
            {
                if (filterDescriptor is CompositeFilterDescriptor)
                {
                    var composite = filterDescriptor as CompositeFilterDescriptor;

                    qb.Add(BuildFilter(composite.FilterDescriptors.ToList(), composite.LogicalOperator.ToString()));
                }
                else
                {
                    var filter = filterDescriptor as FilterDescriptor;
                    var index = _paraCols.Count;
                    var paramName = "@" + filter.Member + index;
                    _paraCols.Add(new SqlParameter(paramName, filter.Value));

                    qb.Add(string.Format(_operators[filter.Operator], filter.Member, paramName));
                }
            }

            return "(" + (qb.Count > 0 ? string.Join(string.Format(" {0} ", operand), qb) : "1 = 1") + ")";
        }
        protected virtual string BuildSort<T>()
        {
            var sortList = new List<string>();
            if (DataTableRequest.Sorts != null)
            {
                var sortables = DataTableRequest.Sorts.Where(x => !x.Member.Contains("'") && !x.Member.Contains("-") && !string.IsNullOrEmpty(x.Member));

                if (sortables != null && sortables.Count() > 0)
                {
                    foreach (var sort in sortables)
                    {
                        sortList.Add(string.Format("{0} {1}", sort.Member, sort.SortDirection == ListSortDirection.Ascending ? "ASC" : "DESC"));
                    }
                }
                else
                {
                    var type = typeof(T);
                    var propertyName = type.GetProperties().First().Name;

                    sortList.Add(propertyName + " ASC");
                }
            }
            else
            {
                var type = typeof(T);
                var propertyName = type.GetProperties().First().Name;

                sortList.Add(propertyName + " ASC");
            }

            return string.Join(", ", sortList);
        }
    }
}
