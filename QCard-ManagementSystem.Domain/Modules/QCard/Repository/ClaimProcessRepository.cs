using Dapper;
using Kendo.Mvc.UI;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.QCard.Model;
using QCard.ManagementSystem.Domain.Modules.QCard.Request;
using System.Data;

namespace Modules.QCard.Repository
{
    public partial interface IClaimProcessRepository : IDapperRepository<ClaimProcessModel>
    {
        DataSourceResult GetDataSourceResult(DataSourceRequest request, long pdcId, string claimNo, string serialNo, int status, object parameter = null);
        ClaimProcessDetail GetClaimProcessDetail(long userId, string claimNo);
        ClaimProcessDetail GetClaimProcessVoucherDetail(long userId, string claimNo);
        int UpdateClaimProcess(ClaimProcessRequest request, long userId);
        int VerifyClaimProcess(ClaimProcessVerifyRequest request, int claimId, long userId);
        int UpdatePartClaimQty(ClaimProcessUpdatePartQty request, long userId);
        int ClaimProcessUpdatePartial(ClaimProcessUpdatePartialRequest request, long userId);
    }

    public partial class ClaimProcessRepository(IDbConnection connection, IDbTransaction transaction) : DapperRepository<ClaimProcessModel>(connection, transaction), IClaimProcessRepository
    {
        public DataSourceResult GetDataSourceResult(DataSourceRequest request, long pdcId, string claimNo, string serialNo, int status, object parameter = null)
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
                orderBy = "ISNULL(c.UPDATED_DATE, c.CLAIM_DATE)";
                orderType = "DESC";
            }

            DynamicParameters _dynamicParams = new(new
            {
                Type = "GetPaged",
                ClaimNo = claimNo,
                SerialNo = serialNo,
                PdcId = pdcId,
                Status = status,
                RowSkip = (request.Page - 1) * request.PageSize,
                PageSize = request.PageSize,
                OrderBy = orderBy,
                OrderType = orderType
            });

            var data = Connection.Query<ClaimProcessModel>("USP_GET_CLAIM_PROCESS",
                        _dynamicParams,
                            commandTimeout: 0,
                            commandType: System.Data.CommandType.StoredProcedure
                        ).ToList();

            _dynamicParams = new DynamicParameters(new
            {
                Type = "GetCount",
                ClaimNo = claimNo,
                SerialNo = serialNo,
                PdcId = pdcId,
                Status = status
            });

            int total = Connection.ExecuteScalar<int>("USP_GET_CLAIM_PROCESS",
                _dynamicParams,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return new DataSourceResult { Data = data, Total = total };
        }

        public ClaimProcessDetail GetClaimProcessDetail(long userId, string claimNo)
        {
            // PARAM HEADER (pakai UserId)
            var headerParams = new DynamicParameters();
            headerParams.Add("UserId", userId);
            headerParams.Add("ClaimNo", claimNo);

            var header = Connection.QuerySingleOrDefault<ClaimProcessDetailHeader>(
                "USP_GET_CLAIM_PROCESS_PROGRAM_HEADER",
                param: headerParams,
                commandType: System.Data.CommandType.StoredProcedure
            );

            // PARAM DETAIL (tanpa UserId)
            var detailParams = new DynamicParameters();
            detailParams.Add("ClaimNo", claimNo);

            var programDetail = Connection.Query<ClaimProcessDetailProgram>(
                "USP_GET_CLAIM_PROCESS_PROGRAM_DETAIL",
                param: detailParams,
                commandType: System.Data.CommandType.StoredProcedure,
                commandTimeout: 0
            ).ToList();

            return new ClaimProcessDetail
            {
                Header = header,
                ProgramDetail = programDetail
            };
        }

        public ClaimProcessDetail GetClaimProcessVoucherDetail(long userId, string claimNo)
        {
            // PARAM HEADER (pakai UserId)
            var headerParams = new DynamicParameters();
            headerParams.Add("UserId", userId);
            headerParams.Add("ClaimNo", claimNo);

            var header = Connection.QuerySingleOrDefault<ClaimProcessDetailHeader>(
                "USP_GET_CLAIM_PROCESS_VOUCHER_HEADER",
                param: headerParams,
                commandType: System.Data.CommandType.StoredProcedure
            );

            // PARAM DETAIL (tanpa UserId)
            var detailParams = new DynamicParameters();
            detailParams.Add("ClaimNo", claimNo);

            var programDetail = Connection.Query<ClaimProcessDetailProgram>(
                "USP_GET_CLAIM_PROCESS_VOUCHER_DETAIL",
                param: detailParams,
                commandType: System.Data.CommandType.StoredProcedure,
                commandTimeout: 0
            ).ToList();

            return new ClaimProcessDetail
            {
                Header = header,
                ProgramDetail = programDetail
            };
        }

        public int UpdateClaimProcess(ClaimProcessRequest request, long userId)
        {
            var isValid = false;

            var tvp = new DataTable();
            tvp.Columns.Add("CLAIM_NO", typeof(string));
            tvp.Columns.Add("CLAIM_DETAIL_ID", typeof(long));
            tvp.Columns.Add("PROGRAM_ID", typeof(long));
            tvp.Columns.Add("PROGRAM_CODE", typeof(string));
            tvp.Columns.Add("PROGRAM_NAME", typeof(string));
            tvp.Columns.Add("PART_NO", typeof(string));
            tvp.Columns.Add("PART_NAME", typeof(string));
            tvp.Columns.Add("TOTAL_CLAIM_QTY", typeof(int));
            tvp.Columns.Add("AVAILABLE_QTY", typeof(int));
            tvp.Columns.Add("BACK_ORDER_QTY", typeof(int));

            foreach (var item in request.ProgramDetails)
            {
                tvp.Rows.Add(
                    item.CLAIM_NO,
                    item.CLAIM_DETAIL_ID,
                    item.PROGRAM_ID,
                    item.PROGRAM_CODE,
                    item.PROGRAM_NAME,
                    item.PART_NO,
                    item.PART_NAME,
                    item.TOTAL_CLAIM_QTY,
                    item.AVAILABLE_QTY,
                    item.BACK_ORDER_QTY
                );
            }

            var parameters = new DynamicParameters();
            parameters.Add("@ClaimId", request.ClaimId);
            parameters.Add("@ActionName", request.ActionName);
            parameters.Add("@ClaimStatus", request.ClaimStatus);
            parameters.Add("@DnNumber", request.DnNumber);
            parameters.Add("@PgiNo", request.PgiNo);
            parameters.Add("@SpbNo", request.SpbNo);
            parameters.Add("@ResiNo", request.ResiNo);
            parameters.Add("@ShippingAgent", request.ShippingAgent);
            parameters.Add("@GrandTotal", request.Grand_Total);
            parameters.Add("@RejectReason", request.ReasonReject);
            parameters.Add("@SpbNotes", request.NOTES);
            parameters.Add("@UserId", userId);
            parameters.Add("@ProgramDetails", tvp.AsTableValuedParameter("ClaimProcessProgramDetailType"));
            parameters.Add("@IsValid", isValid, DbType.Boolean, ParameterDirection.Output);

            Connection.Execute(
                "USP_CLAIM_PROCESS_UPDATE",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            isValid = parameters.Get<bool>("@IsValid");

            return isValid ? 1 : 0;
        }

        public int VerifyClaimProcess(ClaimProcessVerifyRequest request, int claimId, long userId)
        {
            var isValid = false;

            var tvp = new DataTable();
            tvp.Columns.Add("PART_ID", typeof(long));
            tvp.Columns.Add("CLAIM_QTY", typeof(int));
            tvp.Columns.Add("DISCOUNT", typeof(int));
            tvp.Columns.Add("CREATED_BY", typeof(long));

            foreach (var item in request.PART_LIST)
            {
                tvp.Rows.Add(
                    long.Parse(item.PART_ID ?? "0"),
                    item.QTY,
                    item.DISCOUNT,
                    userId
                );
            }
            var parameters = new DynamicParameters();
            parameters.Add("@ClaimId", claimId);
            parameters.Add("@UserId", userId);
            parameters.Add("@Details", tvp.AsTableValuedParameter("ClaimVoucherDetailType"));
            parameters.Add("@IsValid", isValid, DbType.Boolean, ParameterDirection.Output);

            Connection.Execute(
                "USP_CLAIM_PROCESS_COMPLETE_VERIFICATION",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            isValid = parameters.Get<bool>("@IsValid");

            return isValid ? 1 : 0;
        }

        public int UpdatePartClaimQty(ClaimProcessUpdatePartQty request, long userId)
        {
            var isValid = false;

            var tvp = new DataTable();
            tvp.Columns.Add("CLAIM_NO", typeof(string));
            tvp.Columns.Add("CLAIM_DETAIL_ID", typeof(long));
            tvp.Columns.Add("PROGRAM_ID", typeof(long));
            tvp.Columns.Add("PROGRAM_CODE", typeof(string));
            tvp.Columns.Add("PROGRAM_NAME", typeof(string));
            tvp.Columns.Add("PART_NO", typeof(string));
            tvp.Columns.Add("PART_NAME", typeof(string));
            tvp.Columns.Add("TOTAL_CLAIM_QTY", typeof(int));
            tvp.Columns.Add("AVAILABLE_QTY", typeof(int));
            tvp.Columns.Add("BACK_ORDER_QTY", typeof(int));

            foreach (var item in request.ProgramDetails)
            {
                tvp.Rows.Add(
                    item.CLAIM_NO,
                    item.CLAIM_DETAIL_ID,
                    item.PROGRAM_ID,
                    item.PROGRAM_CODE,
                    item.PROGRAM_NAME,
                    item.PART_NO,
                    item.PART_NAME,
                    item.TOTAL_CLAIM_QTY,
                    item.AVAILABLE_QTY,
                    item.BACK_ORDER_QTY
                );
            }

            var parameters = new DynamicParameters();
            parameters.Add("@ClaimNo", request.ClaimNo);
            parameters.Add("@DeliveryNoteNo", request.DnNumber);
            parameters.Add("@UserId", userId);
            parameters.Add("@ProgramDetails", tvp.AsTableValuedParameter("ClaimProcessProgramDetailType"));
            parameters.Add("@IsValid", isValid, DbType.Boolean, ParameterDirection.Output);

            Connection.Execute(
                "USP_CLAIM_PROCESS_UPDATE_QTY",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            isValid = parameters.Get<bool>("@IsValid");

            return isValid ? 1 : 0;
        }

        public int ClaimProcessUpdatePartial(ClaimProcessUpdatePartialRequest request, long userId)
        {
            var isValid = false;

            var parameters = new DynamicParameters();
            parameters.Add("@Type", request.Type);
            parameters.Add("@ClaimId", request.ClaimId);
            parameters.Add("@PgiNo", request.PgiNo);
            parameters.Add("@SpbNo", request.SpbNo);
            parameters.Add("@ResiNo", request.ResiNo);
            parameters.Add("@ShippingAgent", request.ShippingAgent);
            parameters.Add("@SpbNotes", request.NOTES);
            parameters.Add("@UserId", userId);
            parameters.Add("@IsValid", isValid, DbType.Boolean, ParameterDirection.Output);

            Connection.Execute(
                "USP_CLAIM_PROCESS_UPDATE_PARTIAL",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            isValid = parameters.Get<bool>("@IsValid");

            return isValid ? 1 : 0;
        }
    }
}