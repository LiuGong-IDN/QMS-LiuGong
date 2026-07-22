using Dapper;
using HandlebarsDotNet;
using Kendo.Mvc.UI;
using Modules.MyMachines.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using System.Data;
using System.Reflection.PortableExecutable;
using CommandType = System.Data.CommandType;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface IDataSourceRepository : IDapperRepository<BasicDataResponse>
    {
        List<CustomerCode> GetCustomerList(string customer, string filter);
        List<Role> GetRoleList(string filter);
        List<AssetType> GetAssetType(string filter);
        List<ModelMachine> GetModelList(string filter);
        List<ModelMachine> GetModelbyAssetTypeList(long assetTypeId, string filter);
        List<PartNo> GetPartNoList(string filter);
        List<PartNo> GetPartNoById(string partId);
        List<Priority> GetPriorityList(string filter);
        List<Category> GetProgramCategoryList(string filter);
        List<ServiceParty> GetServicePartyList(string filter);
        List<ProgramModel> GetProgramList(string filter);
        List<ProgramModel> GetProgramListByModelId(string filter, long modelId);
        List<PDC> GetPDC(string filter);
        List<Config> GetProgramCategory(string filter);
        List<ShippingAgent> GetShippingAgent(string filter);
        List<Machines> GetSerialNoQCard(string filter, int customerId, string serialno);
        string GetFileImage(string fileType, string fileCategory, string fileCategoryId);
        ActionName GetActionName(string systemType);
        List<ClaimStatus> GetClaimStatusList(string filter);
        List<ClaimProcessStatus> GetClaimProcessStatusList(string filter, int status);
        DataSourceResult GetClaimStatusHistory(DataSourceRequest request, int claimId);
    }
    public partial class DataSourceRepository(IDbConnection connection, IDbTransaction transaction) : DapperRepository<BasicDataResponse>(connection, transaction), IDataSourceRepository
    {
        public List<CustomerCode> GetCustomerList(string customer, string filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Filter", filter);

            List<CustomerCode> result = Connection.Query<CustomerCode>("USP_GET_CUSTOMER", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }
        public List<ClaimStatus> GetClaimStatusList(string filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Filter", filter);

            List<ClaimStatus> result = Connection.Query<ClaimStatus>("USP_GET_CLAIM_STATUS", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }
        public List<ClaimProcessStatus> GetClaimProcessStatusList(string filter, int status)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Filter", filter);
            parameters.Add("@Status", status);

            List<ClaimProcessStatus> result = Connection.Query<ClaimProcessStatus>("USP_GET_STATUS_BY_CLAIM_STATUS", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }
        public List<AssetType> GetAssetTypeList(string assettype, string filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Filter", filter);

            List<AssetType> result = Connection.Query<AssetType>("USP_GET_ASSET_TYPE", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }
        public List<ModelMachine> GetModelList(string model, string filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Filter", filter);

            List<ModelMachine> result = Connection.Query<ModelMachine>("USP_GET_MODEL", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }
        public List<Role> GetRoleList(string filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Filter", filter);

            List<Role> result = Connection.Query<Role>("USP_GET_ROLE", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }
        public List<AssetType> GetAssetType(string filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Filter", filter);

            List<AssetType> result = Connection.Query<AssetType>("USP_GET_ASSET_TYPE", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }
        public List<ModelMachine> GetModelList(string filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Filter", filter);

            List<ModelMachine> result = Connection.Query<ModelMachine>("USP_GET_MODEL", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }
        public List<PartNo> GetPartNoList(string filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Filter", filter);

            List<PartNo> result = Connection.Query<PartNo>("USP_GET_PARTNO", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }
        public List<PartNo> GetPartNoById(string partId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Id", partId);

            List<PartNo> result = Connection.Query<PartNo>("USP_GET_PARTNO", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }
        public List<ModelMachine> GetModelbyAssetTypeList(long assetTypeId, string filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@AssetTypeID", assetTypeId);
            parameters.Add("@Filter", filter);

            List<ModelMachine> result = Connection.Query<ModelMachine>("USP_GET_MODEL_BY_ASSET_TYPE", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }
        public List<Priority> GetPriorityList(string filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Filter", filter);

            List<Priority> result = Connection.Query<Priority>("USP_GET_PRIORITY", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }
        public List<Category> GetProgramCategoryList(string filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Filter", filter);

            List<Category> result = Connection.Query<Category>("USP_GET_CATEGORY_PROGRAM", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }
        public List<ServiceParty>GetServicePartyList(string filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Filter", filter);

            List<ServiceParty> result = Connection.Query<ServiceParty>("USP_GET_SERVICE_PARTY", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }
        public List<ProgramModel> GetProgramList(string filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Filter", filter);

            List<ProgramModel> result = Connection.Query<ProgramModel>("USP_GET_PROGRAM_LIST", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }
        public List<ProgramModel> GetProgramListByModelId(string filter, long modelId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Filter", filter);
            parameters.Add("@ModelId", modelId);

            List<ProgramModel> result = Connection.Query<ProgramModel>("USP_GET_PROGRAM_LIST", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }

        public List<PDC> GetPDC(string filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Filter", filter);

            List<PDC> result = Connection.Query<PDC>("USP_GET_PDC", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }

        public List<Config> GetProgramCategory(string filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Filter", filter);

            List<Config> result = Connection.Query<Config>("USP_GET_PROGRAM_CATEGORY", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }

        public List<ShippingAgent> GetShippingAgent(string filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Filter", filter);

            List<ShippingAgent> result = Connection.Query<ShippingAgent>("USP_GET_SHIPPING_AGENT", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }
        public List<Machines> GetSerialNoQCard(string filter, int customerId, string serialno)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@Filter", filter);
            parameters.Add("@CustomerId", customerId);
            parameters.Add("@SerialNo", serialno);

            List<Machines> result = Connection.Query<Machines>("USP_GET_SERIALNO_QCARD", parameters, null, commandTimeout: 0, commandType: CommandType.StoredProcedure).ToList();

            return result;
        }
        

        public string GetFileImage(string fileType, string fileCategory, string fileCategoryId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@fileType", fileType);
            parameters.Add("@fileCategory", fileCategory);
            parameters.Add("@fileCategoryId", fileCategoryId);

            FilePath? filePath = Connection.Query<FilePath>(
                "USP_GET_FILE_PATH",
                parameters,
                null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            ).FirstOrDefault();

            // pastikan filePath tidak null agar tidak error
            var result = filePath != null
                ? Path.Combine(filePath.DIRECTORY, filePath.FILE_NAME)
                : null;

            return result;
        }
        public ActionName GetActionName(string systemType)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@SystemType", systemType);

            ActionName? result = Connection.Query<ActionName>(
                "USP_GET_ACTION_NAME",
                parameters,
                null,
                commandTimeout: 0,
                commandType: CommandType.StoredProcedure
            ).FirstOrDefault();

            return result;
        }
        public DataSourceResult GetClaimStatusHistory(DataSourceRequest request, int claimId)
        {
            string orderBy = string.Empty;
            string orderType = string.Empty;
            if (request.Sorts.Count > 0)
            {
                if (request.Sorts[0].SortDirection.ToString() == "Descending")
                {
                    orderBy = request.Sorts[0].Member;
                    orderType = "DESC";
                }
                else if (request.Sorts[0].SortDirection.ToString() == "Ascending")
                {
                    orderBy = request.Sorts[0].Member;
                    orderType = "ASC";
                }
            }
            else
            {
                orderBy = "aph.CREATED_DATE";
                orderType = "DESC";
            }
            DynamicParameters _dynamicParams = new(new
            {
                ClaimId = claimId,
                OrderBy = orderBy,
                OrderType = orderType
            });


            var data = Connection.Query<ClaimStatusHistory>(
                "USP_GET_CLAIM_STATUS_HISTORY_BY_CLAIM_ID",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            var total = data.Count;

            return new DataSourceResult { Data = data, Total = total };
        }

    }
}
