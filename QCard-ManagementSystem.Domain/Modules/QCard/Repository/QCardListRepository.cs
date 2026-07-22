using System.Data;
using System.Text.Json;
using Dapper;
using Kendo.Mvc.UI;
using Modules.MyMachines.Model;
using Modules.QCard.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.QCard.Model;
using QCard.ManagementSystem.Domain.Modules.QCard.Request;
using QCard.ManagementSystem.Domain.Modules.QCard.ViewModel;
using CommandType = System.Data.CommandType;


namespace QCard.ManagementSystem.Domain.Modules.QCard.Repository
{
    public partial interface IQCardListRepository : IDapperRepository<QCardListView>
    {
        DataSourceResult GetDataSourceResult(DataSourceRequest request, object parameter = null);
        List<QCardListView> ListQCard(string type, string id, string cardNo);
        QCardPrintModel GetSelectedQCardPrint(string json);
        List<QCardModel> CheckQCardByCardNo(string cardNo);
        QCardModel GetQCardDetail(int id);
        int Save(QCardRequest qcard, long userId);
        int SaveTopup(TopupRequest topup, long userId);
        int Delete(string id);
        DataSourceResult GetUploadSerialNoTemporary(DataSourceRequest request, object parameter = null);
        int ValidateUploadProcess(List<SerialNoUpload> data, string by, out string tempCode, out string msgError);
        DataSourceResult GetUploadSerialNoValidation(DataSourceRequest request, string tempCode, object parameter = null);
        int SubmitFinalUpload(string tempCode, string username, out string msgError);
        List<UploadedSerialNo> GetUploadedSerialNo(string tempCode);
        List<SerialNoUploadResponse> GetUploadValidationByTempCode(string tempCode);
        DataSourceResult GetSerialNoQCard(DataSourceRequest request, int cardId);
        string GetPdfOrientationFromDb();
        List<long> GetAllQCardIds();
        List<QCardExpiryInfo> GetQCardExpiryInfo();
        List<ApprovalEmail> GetEmailListByApprovalLevelAsync(int approvalLevelId);
        List<TopupInfo> GetTopupApprovalInfo(long userId, string serialNo);

    }

    public partial class QCardListRepository(IDbConnection connection, IDbTransaction transaction) : DapperRepository<QCardListView>(connection, transaction), IQCardListRepository
    {
        public DataSourceResult GetDataSourceResult(DataSourceRequest request, object parameter = null)
        {
            var genericDataTableQuery = new GenericDataSourceQuery(Connection, request);
            var output = genericDataTableQuery.GetData<QCardListView>();

            return output;
        }
        public List<long> GetAllQCardIds()
        {
            var param = new DynamicParameters();
            param.Add("@Type", "getallids");

            List<long> result = [.. Connection.Query<long>(
                "USP_GET_ALL_QCARD",
                param: param,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];

            return result;
        }
        public DataSourceResult GetSerialNoQCard(DataSourceRequest request, int cardId)
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
                orderBy = "qd.CREATED_DATE";
                orderType = "DESC";
            }

            DynamicParameters _dynamicParams = new(new
            {
                Type = "GetList",
                CardId = cardId,
                OrderBy = orderBy,
                OrderType = orderType
            });

            var data = Connection.Query<SerialNoQCard>(
                "USP_GET_SERIALNO_QCARD_VIEW",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            var total = data.Count;

            return new DataSourceResult { Data = data, Total = total };
        }
        public QCardPrintModel GetSelectedQCardPrint(string json)
        {
            var param = new DynamicParameters();
            param.Add("@Json", json);

            var qcard = Connection.Query<QcardData>(
                "USP_GET_SELECTED_QCARD_PRINT",
                param,
                commandType: CommandType.StoredProcedure
            ).ToList();

            QCardPrintModel result = new QCardPrintModel()
            {
                qcardDatas = qcard
            };

            return result;
        }
        public List<QCardExpiryInfo> GetQCardExpiryInfo()
        {
            var result = Connection.Query<QCardExpiryInfo>(
                "USP_GET_QCARD_FOR_EXPIRY_NOTIFICATION",
                commandType: CommandType.StoredProcedure
            ).ToList();

            return result;
        }
        public QCardModel GetQCardDetail(int id)
        {
            DynamicParameters _dynamicParams = new(new
            {
                Type = "GetList",
                CardId = id
            });

            var result = Connection.QuerySingleOrDefault<QCardModel>(
                "USP_GET_QCARD_DETAIL_HEADER",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return result;
        }
        public List<QCardListView> ListQCard(string type, string id, string cardNo)
        {
            var GetListParams = new DynamicParameters();
            GetListParams.Add("@Type", type);
            GetListParams.Add("@Id", id);
            GetListParams.Add("@QCard", cardNo);

            List<QCardListView> result = [.. Connection.Query<QCardListView>(
                "USP_GET_QCARD_LIST",
                param: GetListParams,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];
            return result;
        }
        public int Save(QCardRequest qcard, long userId)
        {
            var isValid = false;

            var parameters = new DynamicParameters();
            parameters.Add("@ID", qcard.ID);
            parameters.Add("@CustomerID", qcard.CUSTOMER_ID);
            parameters.Add("@CardNo", qcard.CARD_NO);
            parameters.Add("@ValidToMonth", qcard.VALID_TO_MONTH);
            parameters.Add("@ValidToYear", qcard.VALID_TO_YEAR);
            parameters.Add("@CreatedBy", userId);
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);

            Connection.Execute("USP_QCARD_SAVE", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }
        public int SaveTopup(TopupRequest topup, long userId)
        {
            var isValid = false;
            var parameters = new DynamicParameters();

            parameters.Add("@ID", topup.ID);
            parameters.Add("@CardID", topup.CARD_ID);
            parameters.Add("@CreatedBy", userId);

            // Build string untuk @Details → "SN001,SN002,SN003"
            string detailParam = "";
            if (topup.DETAILS != null && topup.DETAILS.Any())
            {
                detailParam = string.Join(",",
                    topup.DETAILS.Select((d, index) => $"{index + 1}:{d.SERIAL_NO}")
                );
            }

            parameters.Add("@Details", detailParam);
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);

            Connection.Execute(
                "USP_TOPUP_SAVE",
                parameters,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            );

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }

        public int Delete(string id)
        {
            var isValid = false;
                
            var parameters = new DynamicParameters();
            parameters.Add("@ID", id);
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);

            Connection.Execute("USP_QCARD_DELETE", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }
        public List<QCardModel> CheckQCardByCardNo(string cardNo)
        {
            var GetListParams = new DynamicParameters();
            GetListParams.Add("@Type", "GetList");
            GetListParams.Add("@QCard", cardNo);

            List<QCardModel> result = [.. Connection.Query<QCardModel>(
                "USP_GET_QCARD_LIST",
                param: GetListParams,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];
            return result;
        }
        public DataSourceResult GetUploadSerialNoTemporary(DataSourceRequest request, object parameter = null)
        {
            var genericDataTableQuery = new GenericDataSourceQuery(Connection, request);
            var output = genericDataTableQuery.GetData<SerialNoUpload>();

            return output;
        }

        public DataSourceResult GetUploadSerialNoValidation(DataSourceRequest request, string tempCode, object parameter = null)
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
                orderBy = "CREATED_DATE";
                orderType = "DESC";
            }

            DynamicParameters _dynamicParams = new(new
            {
                Type = "GetPaged",
                TempCode = tempCode,
                RowSkip = (request.Page - 1) * request.PageSize,
                PageSize = request.PageSize,
                OrderBy = orderBy,
                OrderType = orderType
            });

            var data = Connection.Query<SerialNoUploadResponse>("USP_SERIAL_NO_GET_UPLOAD_VALIDATION",
                        _dynamicParams,
                            commandTimeout: 30,
                            commandType: System.Data.CommandType.StoredProcedure
                        ).ToList();

            _dynamicParams = new DynamicParameters(new
            {
                Type = "GetCount",
                TempCode = tempCode
            });

            int total = Connection.ExecuteScalar<int>("USP_SERIAL_NO_GET_UPLOAD_VALIDATION",
                    _dynamicParams,
                        commandTimeout: 30,
                        commandType: System.Data.CommandType.StoredProcedure
                    );

            return new DataSourceResult { Data = data, Total = total };
        }
        public int SubmitFinalUpload(string tempCode, string username, out string msgError)
        {
            var isValid = false;
            msgError = string.Empty;

            var parameters = new DynamicParameters();
            parameters.Add("@TempCode", tempCode);
            parameters.Add("@IsValid", isValid, DbType.Boolean, ParameterDirection.Output);
            parameters.Add("@MsgError", msgError, DbType.String, ParameterDirection.Output, 4000);

            Connection.Execute("USP_SERIAL_NO_UPLOAD_SUBMIT", parameters, Transaction, commandType: CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            msgError = parameters.Get<string>("MsgError");

            return isValid ? 1 : 0;
        }
        public int ValidateUploadProcess(List<SerialNoUpload> data, string by, out string tempCode, out string msgError)
        {
            bool isValid = false;
            msgError = string.Empty;
            tempCode = DateTime.Now.ToString("ddMMyyyyHHmmss");
            var json = JsonSerializer.Serialize(data);

            var parameters = new DynamicParameters();
            parameters.Add("@TempCode", tempCode);
            parameters.Add("@Data", json);
            parameters.Add("@By", by);
            parameters.Add("@IsValid", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@MsgError", dbType: DbType.String, direction: ParameterDirection.Output, size: 4000);

            Connection.Execute("USP_SERIALNO_UPLOAD", parameters, Transaction, commandType: CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("@IsValid");
            msgError = parameters.Get<string>("@MsgError");

            return isValid ? 1 : 0;
        }
        public List<UploadedSerialNo> GetUploadedSerialNo(string tempCode)
        {
            DynamicParameters _dynamicParams = new(new
            {
                TempCode = tempCode
            });

            var result = Connection.Query<UploadedSerialNo>(
                "USP_GET_UPLOADED_SERIAL_NO",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return result;
        }
        public List<SerialNoUploadResponse> GetUploadValidationByTempCode(string tempCode)
        {
            DynamicParameters _dynamicParams = new(new
            {
                TempCode = tempCode
            });

            var data = Connection.Query<SerialNoUploadResponse>("USP_GET_UPLOADED_SERIALNO_DOWNLOAD",
                        _dynamicParams,
                            commandTimeout: 30,
                            commandType: System.Data.CommandType.StoredProcedure
                        ).ToList();

            return data;
        }
        public string GetPdfOrientationFromDb()
        {
            const string sql = "SELECT SYSTEM_VALUE_TEXT FROM TB_M_SYSTEM WHERE SYSTEM_TYPE = 'Q-Card'";
            using (var connection = Connection)
            {
                return connection.QueryFirstOrDefault<string>(sql);
            }
        }
        public List<ApprovalEmail> GetEmailListByApprovalLevelAsync(int approvalLevelId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetEmailApproval");
            parameters.Add("@APPROVAL_LEVEL_ID", approvalLevelId);

            var result = Connection.Query<ApprovalEmail>(
                "USP_GET_USER",
                param: parameters,
                transaction: Transaction,
                commandTimeout: 0,
                commandType: CommandType.StoredProcedure
            ).ToList();

            return result?.ToList() ?? new List<ApprovalEmail>();
        }

        public List<TopupInfo> GetTopupApprovalInfo(long userId, string serialNo)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@CreatedBy", userId);
            parameters.Add("@SerialNo", serialNo);


            return Connection.Query<TopupInfo>(
                "USP_GET_TOPUP_INFO",
                param: parameters,
                transaction: Transaction,
                commandTimeout: 0,
                commandType: CommandType.StoredProcedure
            ).ToList();
        }
    }
}