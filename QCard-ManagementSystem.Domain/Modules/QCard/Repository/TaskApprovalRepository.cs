using System.Data;
using Dapper;
using Kendo.Mvc.UI;
using Modules.QCard.Model;
using QCard.ManagementSystem.Domain.Modules.QCard.Model;
using QCard.ManagementSystem.Domain.Modules.QCard.Request;
using QCard.ManagementSystem.Domain.Modules.QCard.ViewModel;
using CommandType = System.Data.CommandType;


namespace QCard.ManagementSystem.Domain.Modules.QCard.Repository
{
    public partial interface ITaskApprovalRepository : IDapperRepository<TaskApprovalView>
    {
        DataSourceResult GetDataSourceResult(DataSourceRequest request, string cardNo, int customer, long userId, object parameter = null);
        List<ApprovalList> ListApproval(string type, int id, string cardNo);
        ApprovalDetail GetTopupDetail(int id);
        ApprovalHistoryDetail GetApprovalHistoryDetail(long approvalHistoryId);
        DataSourceResult GetApprovalHistory(DataSourceRequest request, int documentId);
        List<ApprovalDetail> GetDataDownload();
        int Save(ApprovalRequest approve, long userId);
        int Reject(ApprovalRequest reject, long userId);
        List<ApprovalEmail> GetEmailListByApprovalLevelAsync(int approvalLevelId);
        List<TopupInfo> GetTopupApprovalInfo(long userId, string documentapprovalId);
        List<ApprovalList> GetAllTaskApprovalIds(long userId);
    }

    public partial class TaskApprovalRepository(IDbConnection connection, IDbTransaction transaction) : DapperRepository<TaskApprovalView>(connection, transaction), ITaskApprovalRepository
    {
        public DataSourceResult GetDataSourceResult(DataSourceRequest request, string cardNo, int customer, long userId, object parameter = null)
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
                orderBy = "";
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

                QCard = cardNo,
                Customer = customer,
                UserId = userId
            });

            var data = Connection.Query<ApprovalList>("USP_GET_TASK_APPROVAL",
                _dynamicParams,
                commandTimeout: 30,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            // --- Hitung total record ---
            _dynamicParams = new DynamicParameters(new
            {
                Type = "GetCount",
                QCard = cardNo,
                Customer = customer,
                UserId = userId
            });

            int total = Connection.ExecuteScalar<int>("USP_GET_TASK_APPROVAL",
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

        public List<ApprovalList> ListApproval(string type, int id, string cardNo)
        {
            var GetListParams = new DynamicParameters();
            GetListParams.Add("@Type", type);
            GetListParams.Add("@Id", id);
            GetListParams.Add("@QCard", cardNo);

            List<ApprovalList> result = [.. Connection.Query<ApprovalList>(
                "USP_GET_TASK_APPROVAL",
                param: GetListParams,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];
            return result;
        }

        public List<ApprovalList> GetAllTaskApprovalIds(long userId)
        {
            var param = new DynamicParameters();
            param.Add("@Type", "Getlist");
            param.Add("@UserId", userId);

            var data = Connection.Query<ApprovalList>(
                "USP_GET_TASK_APPROVAL",
                param,
                commandTimeout: 30,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return data;
        }
        public ApprovalDetail GetTopupDetail(int id)
        {
            DynamicParameters _dynamicParams = new(new
            {
                Type = "GetList",
                TopupId = id
            });

            var result = Connection.QuerySingleOrDefault<ApprovalDetail>(
                "USP_GET_APPROVAL_DETAIL_HEADER",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return result;
        }
        public ApprovalHistoryDetail GetApprovalHistoryDetail(long approvalHistoryId)
        {
            DynamicParameters _dynamicParams = new(new
            {
                ApprovalHistoryId = approvalHistoryId
            });
            var data = Connection.QuerySingleOrDefault<ApprovalHistoryDetail>(
                "USP_GET_TASK_APPROVAL_HISTORY_DETAIL",
                param: _dynamicParams,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return data;
        }
        public DataSourceResult GetApprovalHistory(DataSourceRequest request, int documentId)
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
                orderBy = "";
                orderType = "DESC";
            }
            DynamicParameters _dynamicParams = new(new
            {
                OrderBy = orderBy,
                OrderType = orderType,
                DocumentApprovalId = documentId
            });

            var data = Connection.Query<ApprovalHistory>(
                "USP_GET_APPROVAL_HISTORY_VIEW",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            var total = data.Count;

            return new DataSourceResult { Data = data, Total = total };
        }
        public List<ApprovalDetail> GetDataDownload()
        {
            var data = Connection.Query<ApprovalDetail>(
                "USP_DOWNLOAD_ALL_TASK_APPROVAL",
                commandTimeout: 60,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return data;
        }
        public int Save(ApprovalRequest approve, long userId)
        {
            var isValid = false;
            var parameters = new DynamicParameters();

            parameters.Add("@FormTypeId", approve.FORM_TYPE_ID);
            parameters.Add("@ActionName", approve.ACTION_NAME);
            parameters.Add("@UserId", userId);
            parameters.Add("@Evidence", approve.EVIDENCE_FILE_NAME);
            parameters.Add("@Notes", approve.NOTES);

            // Gabungkan semua ID menjadi string CSV: "1,2,3"
            string idsParam = "";
            if (approve.DOCUMENT_APPROVAL_IDS != null && approve.DOCUMENT_APPROVAL_IDS.Any())
            {
                idsParam = string.Join(",", approve.DOCUMENT_APPROVAL_IDS);
            }
            parameters.Add("@DocumentApprovalIds", idsParam);

            parameters.Add("@IsValid", isValid, DbType.Boolean, ParameterDirection.Output);

            Connection.Execute(
                "USP_APPROVAL_TOPUP",
                parameters,
                commandTimeout: 0,
                commandType: CommandType.StoredProcedure
            );

            isValid = parameters.Get<bool>("@IsValid");
            return isValid ? 1 : 0;
        }
        public int Reject(ApprovalRequest reject, long userId)
        {
            var isValid = false;
            var parameters = new DynamicParameters();

            parameters.Add("@FormTypeId", reject.FORM_TYPE_ID);
            parameters.Add("@ActionName", reject.ACTION_NAME);
            parameters.Add("@Evidence", reject.EVIDENCE_FILE_NAME);
            parameters.Add("@UserId", userId);
            parameters.Add("@Notes", reject.NOTES);
            parameters.Add("@Reason", reject.REJECT_REASON);

            // Gabungkan semua ID menjadi string CSV: "1,2,3"
            string idsParam = "";
            if (reject.DOCUMENT_APPROVAL_IDS != null && reject.DOCUMENT_APPROVAL_IDS.Any())
            {
                idsParam = string.Join(",", reject.DOCUMENT_APPROVAL_IDS);
            }
            parameters.Add("@DocumentApprovalIds", idsParam);

            parameters.Add("@IsValid", isValid, DbType.Boolean, ParameterDirection.Output);

            Connection.Execute(
                "USP_REJECT_TOPUP",
                parameters,
                commandTimeout: 0,
                commandType: CommandType.StoredProcedure
            );

            isValid = parameters.Get<bool>("@IsValid");
            return isValid ? 1 : 0;
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

        public List<TopupInfo> GetTopupApprovalInfo(long userId, string documentapprovalId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@CreatedBy", userId);
            parameters.Add("@DocumentApprovalId", documentapprovalId);

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