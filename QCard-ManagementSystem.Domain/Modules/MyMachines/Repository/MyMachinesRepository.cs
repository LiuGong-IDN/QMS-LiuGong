using Dapper;
using Kendo.Mvc.UI;
using Modules.MyMachines.Model;
using Modules.MyMachines.ViewModel;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.MyMachines.Request;
using System.Data;
using System.Data.SqlClient;



namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface IMyMachinesRepository : IDapperRepository<MyMachinesView>
    {
        DataSourceResult GetDataSourceResult(DataSourceRequest request, string cardNo, string serialNo, int model, int machineType, object parameter = null);
        MachinesDetail GetMachinesDetail(int id);
        int Claim(SubmitClaimRequest request, long companyId, long userId);
        int ClaimVoucher(SubmitClaimVoucherRequest request, long companyId, long userId);
        List<MachinesDownload> GetDataDownload(string cardNo);

    }

    public partial class MyMachinesRepository(IDbConnection connection, IDbTransaction transaction) : DapperRepository<MyMachinesView>(connection, transaction), IMyMachinesRepository
    {
        public DataSourceResult GetDataSourceResult(DataSourceRequest request, string cardNo, string serialNo, int model, int machineType, object parameter = null)
        {
            string orderBy = null;
            string orderType = null;

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
                CardNo = cardNo,
                SerialNo = serialNo,
                Model = model,
                MachineType = machineType,
                RowSkip = (request.Page - 1) * request.PageSize,
                PageSize = request.PageSize,
                OrderBy = orderBy,
                OrderType = orderType
            });

            var data = Connection.Query<MyMachinesView>("USP_GET_MY_MACHINES",
                        _dynamicParams,
                            commandTimeout: 30,
                            commandType: System.Data.CommandType.StoredProcedure
                        ).ToList();

            _dynamicParams = new DynamicParameters(new
            {
                Type = "GetCount",
                CardNo = cardNo,
                SerialNo = serialNo,
                MachineType = machineType,
            });

            int total = Connection.ExecuteScalar<int>("USP_GET_MY_MACHINES",
                    _dynamicParams,
                        commandTimeout: 30,
                        commandType: System.Data.CommandType.StoredProcedure
                    );

            return new DataSourceResult { Data = data, Total = total };
        }
        public MachinesDetail GetMachinesDetail(int id)
        {
            DynamicParameters _dynamicParams = new(new
            {
                Type = "GetList",
                MachineId = id
            });

            var result = Connection.QuerySingleOrDefault<MachinesDetail>(
                "USP_GET_MY_MACHINES_DETAIL_HEADER",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return result;
        }
        public int Claim(SubmitClaimRequest request, long companyId, long userId)
        {
            var isValid = false;

            var tvp = new DataTable();
            tvp.Columns.Add("PROGRAM_ID", typeof(long));
            tvp.Columns.Add("CLAIM_QTY", typeof(int));
            tvp.Columns.Add("CREATED_BY", typeof(long));

            foreach (var item in request.SELECTED_PROGRAM)
            {
                tvp.Rows.Add(
                    long.Parse(item.PROGRAM_ID ?? "0"),
                    int.Parse(item.CLAIM_QTY ?? "0"),
                    userId
                );
            }

            var parameters = new DynamicParameters();
            parameters.Add("@CUSTOMER_ID", companyId);
            parameters.Add("@MACHINE_ID", request.MACHINE_ID);
            parameters.Add("@PDC_ID", request.PDC_ID);
            parameters.Add("@CREATED_BY", userId);
            parameters.Add("@Details", tvp.AsTableValuedParameter("ClaimDetailType"));
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);
            Connection.Execute("USP_MY_MACHINE_CLAIM_SUBMIT", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }
        public int ClaimVoucher(SubmitClaimVoucherRequest request, long companyId, long userId)
        {
            var isValid = false;

            var tvp = new DataTable();
            tvp.Columns.Add("PART_ID", typeof(long));
            tvp.Columns.Add("QTY", typeof(int));
            tvp.Columns.Add("DISCOUNT", typeof(int));
            tvp.Columns.Add("CREATED_BY", typeof(long));

            foreach (var item in request.SELECTED_PART)
            {
                tvp.Rows.Add(
                    long.Parse(item.PART_ID ?? "0"),
                    int.Parse(item.QTY ?? "0"),
                    0,
                    userId
                );
            }

            var parameters = new DynamicParameters();
            parameters.Add("@CUSTOMER_ID", companyId);
            parameters.Add("@MACHINE_ID", request.MACHINE_ID);
            parameters.Add("@PDC_ID", request.PDC_ID);
            parameters.Add("@CREATED_BY", userId);
            parameters.Add("@Details", tvp.AsTableValuedParameter("ClaimVoucherDetailType"));
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);
            Connection.Execute("USP_MY_MACHINE_CLAIM_VOUCHER_SUBMIT", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
            //return 1;
        }

        public List<MachinesDownload> GetDataDownload(string cardNo)
        {
            DynamicParameters _dynamicParams = new(new
            {
                CardNo = cardNo
            });

            var data = Connection.Query<MachinesDownload>("USP_DOWNLOAD_MY_MACHINES_LIST",
                _dynamicParams,
                commandTimeout: 30,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return data;
        }


    }
}