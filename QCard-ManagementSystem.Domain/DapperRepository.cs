using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain
{
    public abstract class DapperRepository<T>
    {
        protected IDbTransaction Transaction { get; set; }
        protected IDbConnection Connection { get; set; }

        public DapperRepository(IDbConnection connection, IDbTransaction transaction)
        {
            Connection = connection;
            Transaction = transaction;
        }

        public string TableName
        {
            get
            {
                var type = typeof(T);
                var attribute = type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;

                return attribute.Name;
            }
        }

        internal virtual dynamic Mapping(T item)
        {
            return item;
        }

        public virtual IEnumerable<T> Find(string query, dynamic param)
        {
            IEnumerable<T> items = null;

            var strsql = @"SELECT * FROM {0} WITH(NOLOCK) WHERE {1}";
            strsql = string.Format(strsql, TableName, query);

            items = Connection.Query<T>(strsql, (object)param, Transaction);

            return items;
        }

        public virtual IEnumerable<T> Find(dynamic param)
        {
            return Find(DynamicQuery.GetWhereQuery(param), param);
        }

        public virtual IEnumerable<T> FindAll()
        {
            IEnumerable<T> items = null;

            var strsql = @"SELECT * FROM {0} WITH(NOLOCK)";
            strsql = string.Format(strsql, TableName);

            items = Connection.Query<T>(strsql, Transaction);

            return items;
        }
        //public virtual IEnumerable<T> FindAll(string? overrideTableName = null)
        //{
        //    var tableName = string.IsNullOrWhiteSpace(overrideTableName) ? TableName : overrideTableName;
        //    var sql = $"SELECT * FROM {tableName} WITH(NOLOCK)";
        //    return Connection.Query<T>(sql, transaction: Transaction);
        //}
        public virtual IEnumerable<U> FindAll<U>(string? overrideTableName = null)
        {
            var tableName = string.IsNullOrWhiteSpace(overrideTableName) ? typeof(U).Name : overrideTableName;
            var sql = $"SELECT * FROM {tableName} WITH(NOLOCK)";
            return Connection.Query<U>(sql, transaction: Transaction);
        }
    }
}
