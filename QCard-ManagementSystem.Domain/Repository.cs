using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TAM.WRH.Domain
{
    public interface IRepository<T>
    {
        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        T Add(T item);

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        T Add(T item, string[] columns);

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        int Remove(dynamic param);

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        int Remove(string query, dynamic param);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        int Update(T item);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        int Update(T item, string[] columns);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> Find(dynamic param);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> Find(string query, dynamic param);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> FindAll();
    }

    public abstract class Repository<T> : IRepository<T>
    {
        protected IDbTransaction Transaction { get; set; }
        protected IDbConnection Connection { get; set; }

        public Repository(IDbConnection connection, IDbTransaction transaction)
        {
            Connection = connection;
            Transaction = transaction;
        }

        private string TableName
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

        /// <summary>
        /// Add the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual T Add(T item)
        {
            string keyProperty = GetKeyProperty(item);
            var parameters = (object)Mapping(item);
            var result = Connection.Query<T>(DynamicQuery.GetInsertQuery(TableName, item, keyProperty), parameters, Transaction).FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Add the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual T Add(T item, string[] columns)
        {
            DynamicParameters parameters = new DynamicParameters();
            PropertyInfo[] props = item.GetType().GetProperties();
            string keyProperty = GetKeyProperty(item);

            foreach(var prop in props)
            {
                if(columns.Contains(prop.Name))
                {
                    parameters.Add(prop.Name, prop.GetValue(item));
                }
            }

            var query = DynamicQuery.GetInsertQuery(TableName, parameters, keyProperty, columns);
            var result = Connection.Query<T>(query, parameters, Transaction).FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Update the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual int Update(T item)
        {
            string keyProperty = GetKeyProperty(item);
            var parameters = (object)Mapping(item);
            var result = Connection.Execute(DynamicQuery.GetUpdateQuery(TableName, item, keyProperty), parameters, Transaction);

            return result;
        }

        public virtual int Update(T item, string[] columns)
        {
            string keyProperty = GetKeyProperty(item);
            var parameters = (object)Mapping(item);
            var result = Connection.Execute(DynamicQuery.GetUpdateQuery(TableName, item, keyProperty, columns), parameters, Transaction);

            return result;
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual int Remove(string query, dynamic param)
        {
            var result = Connection.Execute("DELETE FROM " + TableName + " WHERE " + query, (object)param, Transaction);

            return result;
        }

        public virtual int Remove(dynamic param)
        {
            return Remove(DynamicQuery.GetWhereQuery(param), param);
        }

        /// <summary>
        /// Finds the specified predicate.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="param">The param.</param>
        /// <returns>
        /// A list of items
        /// </returns>
        public virtual IEnumerable<T> Find(string query, dynamic param)
        {
            IEnumerable<T> items = null;
            items = Connection.Query<T>("SELECT * FROM " + TableName + " WHERE " + query, (object)param, Transaction);

            return items;
        }

        /// <summary>
        /// Finds the specified param.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <returns></returns>
        public virtual IEnumerable<T> Find(dynamic param)
        {
            return Find(DynamicQuery.GetWhereQuery(param), param);
        }

        /// <summary>
        /// Finds all.
        /// </summary>
        /// <returns>All items</returns>
        public virtual IEnumerable<T> FindAll()
        {
            IEnumerable<T> items = null;
            items = Connection.Query<T>("SELECT * FROM " + TableName, null, Transaction);

            return items;
        }

        private string GetKeyProperty(dynamic item)
        {
            string result = string.Empty;
            PropertyInfo[] properties = item.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                var attribute = Attribute.GetCustomAttribute(property, typeof(KeyAttribute))
                    as KeyAttribute;

                if (attribute != null)
                {
                    result = property.Name;
                }
            }

            return result;
        }

    }
}
