using System.Data;
using System.Text.Json;
using Dapper;
using Kendo.Mvc.UI;
using Modules.MyMachines.Model;
using Modules.MyMachines.ViewModel;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface ISerialNoRepository : IDapperRepository<MachineProgram>
    {
        DataSourceResult GetDataSourceResult(DataSourceRequest request, object parameter = null);
        List<SerialNoView> ListMachine(string type, long id, string machine);
        DataSourceResult GetSerialNoListByMachineId(DataSourceRequest request, int machineId);
        ProgramHeaderDetail GetProgramDetails(int programId);
        int Save(MachineRequest machine, long by);
        MachineDetail GetSerialNoDetail(int id);
        int Delete(string id);
    }

    public partial class SerialNoRepository(IDbConnection connection, IDbTransaction transaction) : DapperRepository<MachineProgram>(connection, transaction), ISerialNoRepository
    {
        public DataSourceResult GetDataSourceResult(DataSourceRequest request, object parameter = null)
        {
            var genericDataTableQuery = new GenericDataSourceQuery(Connection, request);
            var output = genericDataTableQuery.GetData<SerialNoView>();

            return output;
        }
        public DataSourceResult GetSerialNoListByMachineId(DataSourceRequest request, int machineId)
        {
            DynamicParameters _dynamicParams = new(new
            {
                Type = "GetList",
                MachineId = machineId
            });

            var data =  Connection.Query<MachineProgram>(
                "USP_GET_MY_MACHINES",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            var total = data.Count;

            return new DataSourceResult { Data = data, Total = total };
        }
        public ProgramHeaderDetail GetProgramDetails(int programId)
        {
            DynamicParameters _dynamicParams = new(new
            {
                Type = "GetList",
                ProgramId = programId
            });

            List<ProgramModel> programModel = Connection.Query<ProgramModel>(
                "USP_GET_PROGRAM_DETAIL",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            List<ProgramDetail> programDetail = new();
            
            foreach (ProgramModel model in programModel)
            {
                ProgramDetail detail = new();
                detail.PART_NO = model.PART_NO;
                detail.DESCRIPTION = model.DESCRIPTION;
                detail.QTY = model.QTY;
                programDetail.Add(detail);
            }

            ProgramHeaderDetail data = new()
            {
                Header = programModel.FirstOrDefault(),
                Detail = programDetail
            };

            return data;
        }
        public MachineDetail GetSerialNoDetail(int id)
        {
            DynamicParameters _dynamicParams = new(new
            {
                Type = "GetList",
                MachineId = id
            });

            var result = Connection.QuerySingleOrDefault<MachineDetail>(
                "USP_GET_SERIALNO_DETAIL_HEADER",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return result;
        }
        public List<SerialNoView> ListMachine(string type, long id, string machine)
        {
            var GetListParams = new DynamicParameters();
            GetListParams.Add("@Type", type);
            GetListParams.Add("@Id", id);
            GetListParams.Add("@SerialNo", machine);

            List<SerialNoView> result = [.. Connection.Query<SerialNoView>(
                "USP_GET_MY_MACHINES",
                param: GetListParams,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];
            return result;
        }
        public int Save(MachineRequest machine, long by)
        {
            var isValid = false;

            var parameters = new DynamicParameters();
            parameters.Add("@ID", machine.ID);
            parameters.Add("@CustomerID", machine.CUSTOMER_ID);
            parameters.Add("@SerialNo", machine.SERIAL_NO);
            parameters.Add("@PJB_NO", machine.PJB_NO);
            parameters.Add("@Evidence_PJB", machine.EVIDENCE_PJB);
            parameters.Add("@AssetTypeID", machine.ASSET_TYPE_ID);
            parameters.Add("@ModelID", machine.MODEL_ID);
            parameters.Add("@PurchaseDate", machine.PURCHASE_DATE);
            parameters.Add("@ServicePartyID", machine.SERVICE_PARTY_ID);
            parameters.Add("@CreatedBy", by);
            parameters.Add("@Branch", machine.BRANCH);
            parameters.Add("@IsProgram", machine.IS_PROGRAM);
            parameters.Add("@IsVoucher", machine.IS_VOUCHER);
            parameters.Add("@VoucherAmount", machine.VOUCHER_AMOUNT);

            parameters.Add("@IsValid", isValid, DbType.Boolean, ParameterDirection.Output);

            // PROGRAM JSON hanya dikirim jika IS_PROGRAM = true
            string programJson = "[]";
            if (machine.IS_PROGRAM && machine.DETAILS != null && machine.DETAILS.Any())
            {
                var orderedDetails = machine.DETAILS
                    .Select((d, index) => new
                    {
                        ORDER_INDEX = index + 1,
                        d.PROGRAM_ID,
                        d.PACKAGE_QTY,
                        d.CLAIM_QTY,
                        d.CURRENT_PACKAGE_QTY
                    }).ToList();

                programJson = JsonSerializer.Serialize(orderedDetails);
            }

            parameters.Add("@ProgramDetails", programJson);

            Connection.Execute(
                "USP_MACHINE_SAVE",
                parameters,
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

            Connection.Execute("USP_MACHINE_DELETE", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }
    }
}
