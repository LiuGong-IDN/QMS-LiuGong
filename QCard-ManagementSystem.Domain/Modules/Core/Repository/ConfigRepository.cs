using Dapper;
using Kendo.Mvc.UI;
using System.Data;
using QCard.ManagementSystem.Domain.Modules.Core.Model;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface IConfigRepository : IDapperRepository<Config>
    {
        DataSourceResult GetDataSourceResult(DataSourceRequest request, object parameter = null);
        int Save(Config config, string[] columns);
        int Update(Config config, string[] columns);
        int Delete(Config config);
    }

    public partial class ConfigRepository : DapperRepository<Config>, IConfigRepository
    {
        public ConfigRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {
        }

        public DataSourceResult GetDataSourceResult(DataSourceRequest request, object parameter = null)
        {
            var genericDataTableQuery = new GenericDataSourceQuery(Connection, request);
            var output = genericDataTableQuery.GetData<Config>();

            return output;
        }

        public int Save(Config config, string[] columns)
        {
            var ConfigObj = (object)Mapping(config);
            return Connection.Execute(DynamicQuery.GetInsertCompositeQuery("TB_M_SYSTEM", ConfigObj, "SYSTEM_ID", "SYSTEM_TYPE", columns), ConfigObj, Transaction);
        }

        public int Update(Config config, string[] columns)
        {
            var configObj = (object)Mapping(config);
            return Connection.Execute(DynamicQuery.GetUpdateCompositeQuery("TB_M_SYSTEM", configObj, "SYSTEM_ID", "SYSTEM_TYPE", columns), configObj, Transaction);
        }

        public int Delete(Config config)
        {
            var configObj = (object)Mapping(config);
            return Connection.Execute(DynamicQuery.GetHardDeleteCompositeQuery("TB_M_SYSTEM", "SYSTEM_ID", "SYSTEM_TYPE"), configObj, Transaction);
        }
    }
}