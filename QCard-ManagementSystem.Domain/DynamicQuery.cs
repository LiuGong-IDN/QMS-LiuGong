using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain
{
    internal sealed class DynamicQuery
    {
        internal static string GetInsertQuery(string tableName, dynamic item, string keyProperty, string[] columns = null)
        {
            PropertyInfo[] props = item.GetType().GetProperties();
            string[] allcolumns = props.Select(p => p.Name).Where(s => s != keyProperty).ToArray();

            if(columns != null)
            {
                allcolumns = columns;
            }

            var query = string.Format("INSERT INTO {0} ({1}) OUTPUT inserted.{3} VALUES (@{2})",
                                    tableName,
                                    string.Join(",", allcolumns),
                                    string.Join(",@", allcolumns),
                                    keyProperty);
            return query;
        }

        internal static string GetUpdateQuery(string tableName, dynamic item, string keyProperty, string[] columns = null)
        {
            PropertyInfo[] props = item.GetType().GetProperties();
            string[] allcolumns = props.Select(p => p.Name).ToArray();
            string[] excludeColumn = { keyProperty };

            if(columns != null)
            {
                allcolumns = columns;                
            }

            var newcolumns = allcolumns.Except(excludeColumn);
            var parameters = newcolumns.Select(name => name + "=@" + name).ToList();

            var query = string.Format("UPDATE {0} SET {1} WHERE {2} = @{2}", tableName, string.Join(",", parameters), keyProperty);

            return query;
        }

        internal static string GetInsertCompositeQuery(string tableName, dynamic item, string keyProperty, string keyPropertyComposite, string[] columns = null)
        {
            PropertyInfo[] props = item.GetType().GetProperties();
            string[] allcolumns = props.Select(p => p.Name).Where(s => s != keyProperty && s != keyPropertyComposite).ToArray();

            if (columns != null)
            {
                allcolumns = columns;
            }

            var query = string.Format("INSERT INTO {0} ({1}) OUTPUT inserted.{3} VALUES (@{2})",
                                    tableName,
                                    string.Join(",", allcolumns),
                                    string.Join(",@", allcolumns),
                                    keyProperty);
            return query;
        }

        internal static string GetInsertMultipleKeyQuery(string tableName, dynamic item, string[] keyProperties, string[] columns = null)
        {
            PropertyInfo[] props = item.GetType().GetProperties();
            string[] allColumns;

            if (columns != null && columns.Length > 0)
                allColumns = columns;
            else
                allColumns = props.Select(p => p.Name).ToArray();

            string outputClause = string.Join(", ", keyProperties.Select(k => $"inserted.{k}"));

            var query = string.Format("INSERT INTO {0} ({1}) OUTPUT {2} VALUES (@{3})",
                tableName,
                string.Join(",", allColumns),
                outputClause,
                string.Join(",@", allColumns)
            );

            return query;
        }

        internal static string GetUpdateCompositeQuery(string tableName, dynamic item, string keyProperty, string keyPropertyComposite, string[] columns = null)
        {
            PropertyInfo[] props = item.GetType().GetProperties();
            string[] allcolumns = props.Select(p => p.Name).ToArray();
            string[] excludeColumn = { keyProperty };
            string[] excludeColumnSecond = { keyPropertyComposite };

            if (columns != null)
            {
                allcolumns = columns;
            }

            var newcolumns = allcolumns.Except(excludeColumn).Except(excludeColumnSecond);
            var parameters = newcolumns.Select(name => name + "=@" + name).ToList();

            var query = string.Format("UPDATE {0} SET {1} WHERE {2} = @{2} AND {3} = @{3}", tableName, string.Join(",", parameters), keyProperty, keyPropertyComposite);

            return query;
        }

        internal static string GetUpdateMultipleKeyQuery(string tableName, dynamic item, string[] keyProperties, string[] columns = null)
        {
            PropertyInfo[] props = item.GetType().GetProperties();
            string[] allColumns = props.Select(p => p.Name).ToArray();
            if (columns != null)
                allColumns = columns;

            var updateColumns = allColumns.Except(keyProperties);
            var setClause = string.Join(", ", updateColumns.Select(col => $"{col} = @{col}"));
            var whereClause = string.Join(" AND ", keyProperties.Select(key => $"{key} = @{key}"));

            var query = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";
            return query;
        }

        internal static string GetHardDeleteQuery(string tableName, string keyProperty)
        {
            var query = string.Format("DELETE FROM {0} WHERE {1} = @{1}", tableName, keyProperty);

            return query;
        }

        internal static string GetHardDeleteCompositeQuery(string tableName, string keyProperty, string keyPropertyComposite)
        {
            var query = string.Format("DELETE FROM {0} WHERE {1} = @{1} AND {2} = @{2}", tableName, keyProperty, keyPropertyComposite);

            return query;
        }

        internal static string GetHardDeleteMultipleKeyQuery(string tableName, dynamic item, string[] keyProperties)
        {
            string whereClause = string.Join(" AND ", keyProperties.Select(k => $"{k} = @{k}"));

            string query = $"DELETE FROM {tableName} WHERE {whereClause}";

            return query;
        }

        internal static string GetSoftDeleteMultipleKeyQuery(string tableName, string[] keyProperties)
        {
            string whereClause = string.Join(" AND ", keyProperties.Select(k => $"{k} = @{k}"));
            string query = $"UPDATE {tableName} SET IS_DELETED = 1 WHERE {whereClause}";
            return query;
        }

        internal static string GetWhereQuery(dynamic item)
        {
            PropertyInfo[] props = item.GetType().GetProperties();
            string[] columns = props.Select(p => p.Name).ToArray();

            var builder = new StringBuilder();

            for (int i = 0; i < columns.Count(); i++)
            {
                string col = columns[i];
                builder.Append("[" + col + "]");
                builder.Append("=@");
                builder.Append(col);

                if (i < columns.Count() - 1)
                {
                    builder.Append(" AND ");
                }
            }

            return builder.ToString();
        }
    }
}
