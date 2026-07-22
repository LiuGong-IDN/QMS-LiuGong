using System.Data;
using Dapper;
using Kendo.Mvc.UI;
using Modules.MyMachines.Model;
using Modules.MyQCard.Model;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.MyQCard.Model;
using QCard.ManagementSystem.Domain.Modules.MyQCard.Request;
using CommandType = System.Data.CommandType;

namespace Modules.MyQCard.Repository
{
    public partial interface IClaimHistoryRepository : IDapperRepository<ClaimHistoryModel>
    {
        DataSourceResult GetDataSourceResult(DataSourceRequest request, long userId, string claimNo, string serialNo, int status, object parameter = null);

        IEnumerable<ClaimDetail> GetClaimProgramDetail(long userId, int id);
        DataSourceResult GetClaimHistoryProgramDetail(DataSourceRequest request, int claimId);

        IEnumerable<ClaimDetail> GetClaimVoucherDetail(long userId, int id);
        DataSourceResult GetClaimHistoryVoucherDetail(DataSourceRequest request, int claimId);

        int UpdateClaim(UpdateClaimRequest request, long companyId, long userId);
        int UpdateClaimVoucher(UpdateClaimVoucherRequest request, long companyId, long userId);

        int ConfirmPartialPickup(string claimNo, long userId);

        List<ClaimProgramHistoryDetail> GetProgramDetailHistory(int claimId, int claimDetailId, int programId);

        List<ClaimDownload> GetDataDownload(string type, long pdcId, long userId);

        List<PDCEmail> GetEmailListByPdcAsync(int pdcId);
        List<MachineInfo> GetMachineClaimInfo(long customerId, string serialNo, string claimNo, string claimType, long? claimId = null);
    }

    public partial class ClaimHistoryRepository(IDbConnection connection, IDbTransaction transaction) : DapperRepository<ClaimHistoryModel>(connection, transaction), IClaimHistoryRepository
    {
        public DataSourceResult GetDataSourceResult(DataSourceRequest request, long userId, string claimNo, string serialNo, int status, object parameter = null)
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
                UserId = userId,
                Status = status,
                RowSkip = (request.Page - 1) * request.PageSize,
                PageSize = request.PageSize,
                OrderBy = orderBy,
                OrderType = orderType
            });

            var data = Connection.Query<ClaimHistoryModel>("USP_GET_CLAIM_HISTORY",
                        _dynamicParams,
                            commandTimeout: 30,
                            commandType: System.Data.CommandType.StoredProcedure
                        ).ToList();

            _dynamicParams = new DynamicParameters(new
            {
                Type = "GetCount",
                ClaimNo = claimNo,
                SerialNo = serialNo,
                UserId = userId,
                Status = status
            });

            int total = Connection.ExecuteScalar<int>("USP_GET_CLAIM_HISTORY",
                    _dynamicParams,
                        commandTimeout: 30,
                        commandType: System.Data.CommandType.StoredProcedure
                    );

            return new DataSourceResult { Data = data, Total = total };
        }

        public IEnumerable<ClaimDetail> GetClaimProgramDetail(long userId, int id)
        {
            DynamicParameters _dynamicParams = new(new
            {
                UserId = userId,
                ID = id
            });

            var result = Connection.Query<ClaimDetail>(
                "USP_GET_CLAIM_HISTORY_PROGRAM_HEADER",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return result;
        }

        public DataSourceResult GetClaimHistoryProgramDetail(DataSourceRequest request, int claimId)
        {
            DynamicParameters _dynamicParams = new(new
            {
                ClaimId = claimId
            });

            var data = Connection.Query<ClaimProgramDetail>(
                "USP_GET_CLAIM_HISTORY_PROGRAM_DETAIL",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            var total = data.Count;

            return new DataSourceResult { Data = data, Total = total };
        }

        public IEnumerable<ClaimDetail> GetClaimVoucherDetail(long userId, int id)
        {
            DynamicParameters _dynamicParams = new(new
            {
                UserId = userId,
                ID = id
            });

            var result = Connection.Query<ClaimDetail>(
                "USP_GET_CLAIM_HISTORY_VOUCHER_HEADER",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return result;
        }
        public DataSourceResult GetClaimHistoryVoucherDetail(DataSourceRequest request, int claimId)
        {
            DynamicParameters _dynamicParams = new(new
            {
                ClaimId = claimId
            });

            var data = Connection.Query<ClaimVoucherDetail>(
                "USP_GET_CLAIM_HISTORY_VOUCHER_DETAIL",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            var total = data.Count;

            return new DataSourceResult { Data = data, Total = total };
        }

        public int UpdateClaim(UpdateClaimRequest request, long companyId, long userId)
        {
            var isValid = false;

            var tvp = new DataTable();
            tvp.Columns.Add("PROGRAM_ID", typeof(long));
            tvp.Columns.Add("CLAIM_QTY", typeof(int));
            tvp.Columns.Add("CREATED_BY", typeof(long));

            foreach (var item in request.UPDATED_CLAIM)
            {
                tvp.Rows.Add(
                    long.Parse(item.PROGRAM_ID ?? "0"),
                    int.Parse(item.CLAIM_QTY ?? "0"),
                    userId
                );
            }

            var parameters = new DynamicParameters();
            parameters.Add("@CUSTOMER_ID", companyId);
            parameters.Add("@CLAIM_ID", request.CLAIM_ID);
            parameters.Add("@CREATED_BY", userId);
            parameters.Add("@Details", tvp.AsTableValuedParameter("ClaimDetailType"));
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);
            Connection.Execute("USP_CLAIM_HISTORY_UPDATE", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }

        public int UpdateClaimVoucher(UpdateClaimVoucherRequest request, long companyId, long userId)
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
            parameters.Add("@CUSTOMER_ID", companyId);
            parameters.Add("@CLAIM_ID", request.CLAIM_ID);
            parameters.Add("@CREATED_BY", userId);
            parameters.Add("@Details", tvp.AsTableValuedParameter("ClaimVoucherDetailType"));
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);
            Connection.Execute("USP_CLAIM_HISTORY_VOUCHER_UPDATE", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }

        public int ConfirmPartialPickup(string claimNo, long userId)
        {
            var isValid = false;

            var parameters = new DynamicParameters();
            parameters.Add("@ClaimNo", claimNo);
            parameters.Add("@UserId", userId);
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);
            Connection.Execute("USP_CLAIM_HISTORY_PARTIAL_PICKUP", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }

        public List<ClaimProgramHistoryDetail> GetProgramDetailHistory(int claimId, int claimDetailId, int programId)
        {
            List<ClaimProgramHistoryDetail> data;
            if (programId is not 0)
            {
                DynamicParameters _dynamicParams = new(new
                {
                    ClaimId = claimId,
                    ClaimDetailId = claimDetailId,
                    ProgramId = programId
                });

                data = [.. Connection.Query<ClaimProgramHistoryDetail>("USP_GET_CLAIM_PROGRAM_DETAIL_HISTORY",
                        _dynamicParams,
                        commandTimeout: 30,
                        commandType: System.Data.CommandType.StoredProcedure
                        )];
            }
            else
            {
                DynamicParameters _dynamicParams = new(new
                {
                    ClaimId = claimId,
                    ClaimDetailId = claimDetailId
                });

                data = [.. Connection.Query<ClaimProgramHistoryDetail>("USP_GET_CLAIM_VOUCHER_DETAIL_HISTORY",
                        _dynamicParams,
                        commandTimeout: 30,
                        commandType: System.Data.CommandType.StoredProcedure
                        )];
            }

            return data;
        }

        public List<ClaimDownload> GetDataDownload(string type, long pdcId, long userId)
        {
            DynamicParameters _dynamicParams = new(new
            {
                DownloadType = type,
                PDC = pdcId,
                UserId = userId
            });

            var data = Connection.Query<ClaimDownload>("USP_DOWNLOAD_CLAIM_DATA",
                _dynamicParams,
                commandTimeout: 30,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return data;
        }

        public List<PDCEmail> GetEmailListByPdcAsync(int pdcId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetEmailList");
            parameters.Add("@PDC_ID", pdcId.ToString());

            var result = Connection.Query<PDCEmail>(
                "USP_GET_USER",
                param: parameters,
                transaction: Transaction,
                commandTimeout: 0,
                commandType: CommandType.StoredProcedure
            ).ToList();

            return result?.ToList() ?? new List<PDCEmail>();
        }

        public List<MachineInfo> GetMachineClaimInfo(long customerId, string serialNo, string claimNo, string claimType, long? claimId = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetList");
            parameters.Add("@CreatedBy", customerId);
            parameters.Add("@SerialNo", serialNo);
            parameters.Add("@ClaimNo", claimNo);
            parameters.Add("@ClaimType", claimType);
            parameters.Add("@ClaimId", claimId);

            return Connection.Query<MachineInfo>(
                "USP_GET_MACHINE_INFO",
                param: parameters,
                transaction: Transaction,
                commandTimeout: 0,
                commandType: CommandType.StoredProcedure
            ).ToList();
        }
    }
}