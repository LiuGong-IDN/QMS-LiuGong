using System.Data;
using Dapper;
using Kendo.Mvc.UI;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface IModelRepository : IDapperRepository<ModelMachine>
    {
        DataSourceResult GetDataSourceResult(DataSourceRequest request, object parameter = null);
        List<ModelView> ListModel(string type, string id, string modelCode, int assetType);
        int Save(ModelRequest model, long userId);
        int Delete(string id);
        
    }

    public partial class ModelRepository(IDbConnection connection, IDbTransaction transaction) : DapperRepository<ModelMachine>(connection, transaction), IModelRepository
    {
        public DataSourceResult GetDataSourceResult(DataSourceRequest request, object parameter = null)
        {
            var genericDataTableQuery = new GenericDataSourceQuery(Connection, request);
            var output = genericDataTableQuery.GetData<ModelView>();

            return output;
        }
        public List<ModelView> ListModel(string type, string id, string modelCode, int assetType)
        {
            var GetListParams = new DynamicParameters();
            GetListParams.Add("@Type", type);
            GetListParams.Add("@Id", id);
            GetListParams.Add("@ModelName", modelCode);
            GetListParams.Add("@AssetType", assetType);

            List<ModelView> result = [.. Connection.Query<ModelView>(
                "USP_GET_MODEL",
                param: GetListParams,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];
            return result;
        }
        public int Save(ModelRequest model, long userId)
        {
            var isValid = false;

            var parameters = new DynamicParameters();
            parameters.Add("@ID", model.ID);
            parameters.Add("@AssetTypeID", model.ASSET_TYPE_ID);
            parameters.Add("@ModelCode", model.MODEL_CODE);
            parameters.Add("@ModelName", model.MODEL_NAME);
            parameters.Add("@CreatedBy", userId);
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);

            Connection.Execute("USP_MODEL_SAVE", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }
        public int Delete(string id)
        {
            var isValid = false;

            var parameters = new DynamicParameters();
            parameters.Add("@ID", id);
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);

            Connection.Execute("USP_MODEL_DELETE", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }
    }
}
