using System.Data;
using Dapper;
using Kendo.Mvc.UI;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using System.Text.Json;
using CommandType = System.Data.CommandType;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface IProgramRepository : IDapperRepository<MachineProgram>
    {
        DataSourceResult GetDataSourceResult(DataSourceRequest request, object parameter = null);
        List<ProgramView> ListProgram(string type, long id);
        DataSourceResult GetProgramListByMachineId(DataSourceRequest request, int machineId);
        DataSourceResult GetProgramMaterial(DataSourceRequest request, int programId);
        ProgramHeaderDetail GetProgramDetails(int programId);
        ProgramModel GetProgramsDetail(int id);
        ProgramModel GetProgramsDetailEdit(int id);
        int Save(ProgramRequest program);
        int Delete(string id);
        DataSourceResult GetUploadMaterialTemporary(DataSourceRequest request, object parameter = null);
        int ValidateUploadProcess(List<ProgramMaterialUpload> data, string by, out string tempCode, out string msgError);
        DataSourceResult GetUploadMaterialValidation(DataSourceRequest request, string tempCode, object parameter = null);
        int SubmitFinalUpload(string tempCode, string username, out string msgError);
        List<UploadedMaterials> GetUploadedMaterials(string tempCode);
        List<ProgramMaterialUploadResponse> GetUploadValidationByTempCode(string tempCode);
        List<ProgramDetail> GetProgramDetailListByProgramId(int programId);
    }

    public partial class ProgramRepository(IDbConnection connection, IDbTransaction transaction) : DapperRepository<MachineProgram>(connection, transaction), IProgramRepository
    {
        public DataSourceResult GetDataSourceResult(DataSourceRequest request, object parameter = null)
        {
            var genericDataTableQuery = new GenericDataSourceQuery(Connection, request);
            var output = genericDataTableQuery.GetData<ProgramView>();

            return output;
        }
        public DataSourceResult GetProgramListByMachineId(DataSourceRequest request, int machineId)
        {
            DynamicParameters _dynamicParams = new(new
            {
                MachineId = machineId
            });

            var data =  Connection.Query<MachineProgram>(
                "USP_GET_PROGRAM_BY_MACHINEID",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            ).OrderBy(x => x.ORDER_INDEX).ToList();

            var total = data.Count;

            return new DataSourceResult { Data = data, Total = total };
        }
        public ProgramModel GetProgramsDetail(int id)
        {
            DynamicParameters _dynamicParams = new(new
            {
                Type = "GetList",
                ProgramId = id
            });

            var result = Connection.QuerySingleOrDefault<ProgramModel>(
                "USP_GET_PROGRAM_DETAIL_HEADER",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return result;
        }
        public ProgramModel GetProgramsDetailEdit(int id)
        {
            DynamicParameters _dynamicParams = new(new
            {
                Type = "GetList",
                ProgramId = id
            });

            var result = Connection.QuerySingleOrDefault<ProgramModel>(
                "USP_GET_PROGRAM_GET_BY_ID",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return result;
        }
        public List<UploadedMaterials> GetUploadedMaterials(string tempCode)
        {
            DynamicParameters _dynamicParams = new(new
            {
                TempCode = tempCode
            });

            var result = Connection.Query<UploadedMaterials>(
                "USP_GET_UPLOADED_MATERIALS",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return result;
        }
        public ProgramHeaderDetail GetProgramDetails(int programId)
        {
            DynamicParameters _dynamicParams = new(new
            {
                Type = "GetList",
                ProgramId = programId
            });

            List<ProgramModel> programModel = Connection.Query<ProgramModel>(
                "USP_GET_PROGRAM_DETAIL_HEADER",
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
        public DataSourceResult GetProgramMaterial(DataSourceRequest request, int programId)
        {
            DynamicParameters _dynamicParams = new(new
            {
                Type = "GetList",
                ProgramId = programId
            });

            var data = Connection.Query<ProgramMaterial>(
                "USP_GET_PROGRAM_MATERIAL",
                param: _dynamicParams,
                commandType: System.Data.CommandType.StoredProcedure
            ).OrderBy(x => x.ORDER_INDEX).ToList();

            var total = data.Count;

            return new DataSourceResult { Data = data, Total = total };
        }
        public List<ProgramView> ListProgram(string type, long id)
        {
            var GetListParams = new DynamicParameters();
            GetListParams.Add("@Type", type);
            GetListParams.Add("@Id", id);
            //GetListParams.Add("@ProgramCode", program);

            List<ProgramView> result = [.. Connection.Query<ProgramView>(
                "USP_GET_MASTER_PROGRAM",
                param: GetListParams,
                transaction: null,
                commandTimeout: 0,
                commandType: System.Data.CommandType.StoredProcedure
            )];
            return result;
        }
        public int Save(ProgramRequest program)
        {
            var isValid = false;
            var parameters = new DynamicParameters();
            parameters.Add("@ID", program.ID);
            parameters.Add("@ProgramCode", program.PROGRAM_CODE);
            parameters.Add("@ProgramName", program.PROGRAM_NAME);
            parameters.Add("@ModelID", program.MODEL_ID);
            parameters.Add("@PriorityID", program.PRIORITY);
            parameters.Add("@CreatedBy", program.CREATED_BY);
            parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);

            // Build string untuk @Details → "1:PN001:5,2:PN002:10"
            string detailParam = "";
            if (program.DETAILS != null && program.DETAILS.Any())
            {
                detailParam = string.Join(",", program.DETAILS
                    .Select((d, index) => $"{index + 1}:{d.PART_NO}:{d.QTY}"));
            }

            parameters.Add("@Details", detailParam);

            var result = Connection.Execute(
                "USP_PROGRAM_SAVE",
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

            Connection.Execute("USP_PROGRAM_DELETE", parameters, null, commandTimeout: 0, commandType: System.Data.CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            return isValid ? 1 : 0;
        }
        public int ValidateUploadProcess(List<ProgramMaterialUpload> data, string by, out string tempCode, out string msgError)
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

            Connection.Execute("[dbo].[USP_PARTNO_UPLOAD]", parameters, Transaction, commandType: CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("@IsValid");
            msgError = parameters.Get<string>("@MsgError");

            return isValid ? 1 : 0;
        }

        public DataSourceResult GetUploadMaterialTemporary(DataSourceRequest request, object parameter = null)
        {
            var genericDataTableQuery = new GenericDataSourceQuery(Connection, request);
            var output = genericDataTableQuery.GetData<ProgramMaterialUpload>();

            return output;
        }
        public DataSourceResult GetUploadMaterialValidation(DataSourceRequest request, string tempCode, object parameter = null)
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

            var data = Connection.Query<ProgramMaterialUploadResponse>("USP_PART_NO_GET_UPLOAD_VALIDATION",
                        _dynamicParams,
                            commandTimeout: 30,
                            commandType: System.Data.CommandType.StoredProcedure
                        ).ToList();

            _dynamicParams = new DynamicParameters(new
            {
                Type = "GetCount",
                TempCode = tempCode
            });

            int total = Connection.ExecuteScalar<int>("USP_PART_NO_GET_UPLOAD_VALIDATION",
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

            Connection.Execute("[dbo].[USP_PART_NO_UPLOAD_SUBMIT]", parameters, Transaction, commandType: CommandType.StoredProcedure);

            isValid = parameters.Get<bool>("IsValid");
            msgError = parameters.Get<string>("MsgError");

            return isValid ? 1 : 0;
        }
        public List<ProgramMaterialUploadResponse> GetUploadValidationByTempCode(string tempCode)
        {
            DynamicParameters _dynamicParams = new(new
            {
                TempCode = tempCode
            });

            var data = Connection.Query<ProgramMaterialUploadResponse>("USP_GET_UPLOADED_MATERIALS_DOWNLOAD",
                        _dynamicParams,
                            commandTimeout: 30,
                            commandType: System.Data.CommandType.StoredProcedure
                        ).ToList();

            return data;
        }
        public List<ProgramDetail> GetProgramDetailListByProgramId(int programId)
        {
            DynamicParameters _dynamicParams = new(new
            {
                ProgramId = programId
            });

            var data = Connection.Query<ProgramDetail>("USP_GET_PROGRAM_DETAIL_BY_PROGRAM_ID",
                        _dynamicParams,
                            commandTimeout: 30,
                            commandType: System.Data.CommandType.StoredProcedure
                        ).ToList();

            return data;
        }
    }
}
