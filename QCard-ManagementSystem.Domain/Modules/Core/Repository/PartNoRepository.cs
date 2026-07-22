using System.Data;
using System.Text.Json;
using Dapper;
using Kendo.Mvc.UI;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using CommandType = System.Data.CommandType;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface IPartNoRepository : IDapperRepository<PartNo>
    {
        DataSourceResult GetDataSourceResult(DataSourceRequest request, string partNo, object parameter = null);
        List<PartNoView> ListPart(string type, int id, string partNo);
        int Save(PartNoRequest partno, long userId);
        int ValidateUploadProcess(List<PartNoUpload> data, string username, out string tempCode, out string msgError);
        DataSourceResult GetUploadPartNoTemporary(DataSourceRequest request, object parameter = null);
        int SubmitFinalUpload(string tempCode, string username, out string msgError);
        List<PartNoUploadResponse> GetPartNoUploadValidationByTempCode(string tempCode);
        int Delete(string id);
    }

    public partial class PartNoRepository(IDbConnection connection, IDbTransaction transaction) : DapperRepository<PartNo>(connection, transaction), IPartNoRepository
    {
        public DataSourceResult GetDataSourceResult(DataSourceRequest request, string partNo, object parameter = null)
        {
            string orderBy = string.Empty;
            string orderType = string.Empty;

            // --- Sorting ---
            if (request.Sorts != null && request.Sorts.Count > 0)
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
                // default sort
                orderBy = "COALESCE(p.UPDATED_DATE, p.CREATED_DATE)";
                orderType = "DESC";
            }

            // --- Ambil data paged ---
            DynamicParameters _dynamicParams = new(new
            {
                Type = "GetPaged",
                RowSkip = (request.Page - 1) * request.PageSize,
                PageSize = request.PageSize,
                OrderBy = orderBy,
                OrderType = orderType,
                PartNo = partNo,
            });

            var data = Connection.Query<PartNoView>("USP_GET_PARTNO",
                _dynamicParams,
                commandTimeout: 30,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            // --- Hitung total record ---
            _dynamicParams = new DynamicParameters(new
            {
                Type = "GetCount",
                PartNo = partNo,
            });

            int total = Connection.ExecuteScalar<int>("USP_GET_PARTNO",
                _dynamicParams,
                commandTimeout: 30,
                commandType: System.Data.CommandType.StoredProcedure
            );

            // --- Return ke DataSource ---
            return new DataSourceResult
            {
                Data = data,
                Total = total
            };
        }
        public List<PartNoView> ListPart(string type, int id, string partNo)
        {
            var GetListParams = new DynamicParameters();
            GetListParams.Add("@Type", type);
            GetListParams.Add("@Id", id);
            GetListParams.Add("@PartNo", partNo);

            List<PartNoView> result = [.. Connection.Query<PartNoView>(
                "USP_GET_PARTNO",
                param: GetListParams,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];
            return result;
        }
        public int Save(PartNoRequest partno, long userId)
        {
            var isValid = false;

            var parameters = new DynamicParameters();
            parameters.Add("@ID", partno.ID);
            parameters.Add("@PartNo", partno.PART_NO);
            parameters.Add("@PartName", partno.PART_NAME);
            parameters.Add("@RetailPrice", partno.RETAIL_PRICE);
            parameters.Add("@CreatedBy", userId);
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);

            Connection.Execute("USP_PARTNO_SAVE", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }
        public int ValidateUploadProcess(List<PartNoUpload> data, string username, out string tempCode, out string msgError)
        {
            var isValid = false;
            msgError = string.Empty;
            tempCode = DateTime.Now.ToString("ddMMyyyyHHmmss");
            var json = JsonSerializer.Serialize(data);

            var parameters = new DynamicParameters();
            parameters.Add("@TempCode", tempCode);
            parameters.Add("@Data", json);
            parameters.Add("@Username", username);
            parameters.Add("@IsValid", isValid, DbType.Boolean, ParameterDirection.Output);
            parameters.Add("@MsgError", msgError, DbType.String, ParameterDirection.Output, 4000);

            Connection.Execute("[dbo].[USP_MASTER_PART_NO_UPLOAD]", parameters, Transaction, commandType: CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("@IsValid");
            msgError = parameters.Get<string>("@MsgError");

            return isValid ? 1 : 0;
        }
        public DataSourceResult GetUploadPartNoTemporary(DataSourceRequest request, object parameter = null)
        {
            var genericDataTableQuery = new GenericDataSourceQuery(Connection, request);
            var output = genericDataTableQuery.GetData<PartNoUpload>();

            return output;
        }
        public int SubmitFinalUpload(string tempCode, string username, out string msgError)
        {
            var isValid = false;
            msgError = string.Empty;

            var parameters = new DynamicParameters();
            parameters.Add("@TempCode", tempCode);
            parameters.Add("@UserName", username);
            parameters.Add("@IsValid", isValid, DbType.Boolean, ParameterDirection.Output);
            parameters.Add("@MsgError", msgError, DbType.String, ParameterDirection.Output, 4000);

            Connection.Execute("[dbo].[USP_MASTER_PART_NO_UPLOAD_SUBMIT]", parameters, Transaction, commandType: CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            msgError = parameters.Get<string>("MsgError");

            return isValid ? 1 : 0;
        }
        public List<PartNoUploadResponse> GetPartNoUploadValidationByTempCode(string tempCode)
        {
            DynamicParameters _dynamicParams = new(new
            {
                TempCode = tempCode
            });

            var data = Connection.Query<PartNoUploadResponse>("USP_GET_UPLOADED_PART_NO_DOWNLOAD",
                        _dynamicParams,
                            commandTimeout: 30,
                            commandType: System.Data.CommandType.StoredProcedure
                        ).ToList();

            return data;
        }
        public int Delete(string id)
        {
            var isValid = false;

            var parameters = new DynamicParameters();
            parameters.Add("@ID", id);
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);

            Connection.Execute("USP_PARTNO_DELETE", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }
    }
}
