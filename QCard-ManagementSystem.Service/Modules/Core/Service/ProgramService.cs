using ExcelDataReader;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Infrastructure.Helper;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Service;
using QCard.ManagementSystem.Service.Modules.Core.Service;
using QCard.ManagementSystem.Service.Modules.Core.Validator;
using System.Text;
using System.Text.Json;
namespace Modules.Core.Service
{
    public class ProgramService(IDbHelper db, IEmailService emailService, IWebHostEnvironment env) : DbService(db)
    {
        private readonly IEmailService _emailService = emailService;
        private readonly TimeSpan TokenTtl = TimeSpan.FromMinutes(30);
        private readonly IWebHostEnvironment _env = env;
        private readonly string _userEmail = SessionManager.Email;

        public DataSourceResult GetDataSourceResult(DataSourceRequest request)
        {
            string methodName = "GetDataSourceResult";
            var result = new DataSourceResult();
            try
            {
                result = Db.ProgramRepository.GetDataSourceResult(request);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicDataResponse GetProgramsDetail(int id)
        {
            string methodName = "GetProgramsDetail";
            var result = new BasicDataResponse();
            try
            {
                ProgramModel? programsDetail = Db.ProgramRepository.GetProgramsDetail(id);

                result.Data = programsDetail;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Terjadi kesalahan sistem. " + ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicDataResponse GetProgramsDetailEdit(int id)
        {
            string methodName = "GetProgramsDetailEdit";
            var result = new BasicDataResponse();
            try
            {
                ProgramModel? programsDetail = Db.ProgramRepository.GetProgramsDetail(id);

                result.Data = programsDetail;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Terjadi kesalahan sistem. " + ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public DataSourceResult GetProgramListByMachineId(DataSourceRequest request, int machineId)
        {
            string methodName = "GetProgramByMachineId";
            var result = new DataSourceResult();
            try
            {
                result = Db.ProgramRepository.GetProgramListByMachineId(request, machineId);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public ProgramHeaderDetail GetProgramDetails(int programId)
        {
            string methodName = "GetProgramDetails";
            ProgramHeaderDetail result = new();
            try
            {
                result = Db.ProgramRepository.GetProgramDetails(programId);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public DataSourceResult GetProgramMaterial(DataSourceRequest request, int programId)
        {
            string methodName = "GetProgramMaterial";
            var result = new DataSourceResult();
            try
            {
                result = Db.ProgramRepository.GetProgramMaterial(request, programId);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicDataResponse GetUploadedMaterials(string tempCode)
        {
            string methodName = "GetUploadedMaterials";
            var result = new BasicDataResponse();
            try
            {
                List<UploadedMaterials?> uploadedMaterials = Db.ProgramRepository.GetUploadedMaterials(tempCode);

                result.Data = uploadedMaterials;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Terjadi kesalahan sistem. " + ex.Message;
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicResponse Save(ProgramRequest program)
        {
            BasicResponse message = new();

            try
            {
                ProgramValidator validator = new();
                var validate = validator.Validate(program);

                if (!validate.IsValid)
                {
                    message.Success = false;
                    message.Validate = false;
                    message.Message = JsonSerializer.Serialize(validate.Errors);
                    return message;
                }

                var programId = Db.ProgramRepository.Save(program);

                message.Success = true;
                message.Validate = true;
                message.Message = "Data saved successfully";
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.Validate = true;
                message.Message = ex.Message ?? "Failed to save data";
            }

            return message;
        }
        public BasicResponse Delete(string id)
        {
            string methodName = "Delete";
            BasicResponse message = new();
            try
            {
                var result = Db.ProgramRepository.Delete(id);

                if (result > 0)
                {
                    message.Success = true;
                    message.Message = string.Empty;
                }
                else
                {
                    throw new Exception("Failed Delete Data");
                }

                new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
                message.Success = false;
                message.Message = ex.Message;
            }

            return message;
        }
        public ProgramView GetProgramById(long id)
        {
            var empty = string.Empty;
            return Db.ProgramRepository.ListProgram("GetList", id).FirstOrDefault();
        }
        public ExcelResponse HandleExcelUpload(IEnumerable<IFormFile> upload)
        {
            const string methodName = "HandleExcelUpload";
            var materials = new List<ProgramMaterialUpload>();
            ExcelResponse response;
            bool checkHeader = true;

            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using (var stream = new MemoryStream())
                {
                    foreach (var file in upload)
                    {
                        if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                            throw new Exception("File format only support .xlsx");

                        file.CopyTo(stream);
                        stream.Position = 0;

                        using var reader = ExcelReaderFactory.CreateReader(stream);
                        int pos = 0;
                        int step = 5;

                        while (reader.Read())
                        {
                            string itemMessage = "";
                            pos++;

                            if (reader.Depth == 0 || pos <= step || reader.GetValue(0) == null)
                            {
                                if (pos == step)
                                {
                                    if (reader.FieldCount == 4)
                                    {
                                        //Check Header
                                        checkHeader = (reader.GetValue(0).ToString() == "No." &&
                                                       reader.GetValue(1).ToString() == "Part No." &&
                                                       reader.GetValue(2).ToString() == "Part Name" &&
                                                       reader.GetValue(3).ToString() == "Qty") ? true : false;
                                        if (checkHeader == false)
                                        {
                                            throw new Exception("Format Excel Invalid");
                                        }
                                    }
                                    else
                                    {
                                        checkHeader = false;
                                        throw new Exception("Format Excel Invalid");
                                    }
                                }

                                continue;
                            }

                            var itemNO = reader.GetValue(0)?.ToString()?.Trim();
                            var itemPART_NO = reader.GetValue(1)?.ToString()?.Trim();
                            var itemPART_NAME = reader.GetValue(2)?.ToString()?.Trim();
                            var qtyValue = reader.GetValue(3);

                            int itemQTY = 0;

                            // 🔹 Validasi Part No
                            if (string.IsNullOrWhiteSpace(itemPART_NO))
                                itemMessage += "Part No is required, ";

                            // 🔹 Validasi Qty
                            if (qtyValue == null || string.IsNullOrWhiteSpace(qtyValue.ToString()))
                            {
                                itemMessage += "Qty is required, ";
                            }
                            else if (!int.TryParse(qtyValue.ToString().Trim(), out itemQTY))
                            {
                                itemMessage += "Qty must be a valid number, ";
                            }
                            else if (itemQTY < 0)
                            {
                                itemMessage += "Qty cannot be negative, ";
                            }

                            // Tambahkan ke list
                            materials.Add(new ProgramMaterialUpload
                            {
                                PART_NO = itemPART_NO,
                                PART_NAME = itemPART_NAME,
                                QTY = itemQTY,
                                ORDER_INDEX = materials.Count + 1,
                                MESSAGE = itemMessage.TrimEnd(' ', ','), // simpan pesan error jika ada
                                STATUS = string.IsNullOrEmpty(itemMessage) ? "Success" : "Error"
                            });
                        }
                    }
                }

                if (materials.Count == 0)
                    throw new Exception("Your File has no data");

                var result = UploadProcess(materials, out string tempCode, out string msgError);

                response = new ExcelResponse
                {
                    tempCode = tempCode,
                    message = msgError,
                    status = result && string.IsNullOrEmpty(msgError) ? "Success" : "Error",
                    canSubmit = result && string.IsNullOrEmpty(msgError),
                    isSaved = true
                };
            }
            catch (Exception ex)
            {
                response = new ExcelResponse
                {
                    tempCode = string.Empty,
                    message = ex.Message,
                    status = "Error",
                    canSubmit = false,
                    isSaved = false
                };

                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return response;
        }
        public DataSourceResult GetUploadMaterialTemporary(DataSourceRequest request, string tempCode)
        {
            string methodName = "GetUploadMaterialTemporary";
            var result = new DataSourceResult();
            try
            {
                result = Db.ProgramRepository.GetUploadMaterialValidation(request, tempCode);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public bool UploadProcess(List<ProgramMaterialUpload> data, out string tempcode, out string msgerror)
        {
            bool result;
            string methodName = "UploadProcess";
            tempcode = string.Empty;
            string by = string.IsNullOrWhiteSpace(SessionManager.FirstName)
                ? SessionManager.LastName ?? ""
                : $"{SessionManager.FirstName} {SessionManager.LastName}".Trim();

            try
            {
                string currentUser = SessionManager.Current;

                var uploadResult = Db.ProgramRepository.ValidateUploadProcess(data, by, out tempcode, out msgerror);

                if (uploadResult > 0)
                {
                    result = true;
                }
                else
                {
                    throw new Exception(!string.IsNullOrEmpty(msgerror) ? msgerror : "Upload failed.");
                }

                new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
                msgerror = ex.Message;
                result = false;
            }

            return result;
        }
        public ExcelResponse SubmitExcelUpload(string tempCode)
        {
            const string methodName = "SubmitExcelUpload";

            try
            {
                string currentUser = SessionManager.Current;

                var submitResult = Db.ProgramRepository.SubmitFinalUpload(tempCode, currentUser, out string msgError);

                var response = new ExcelResponse
                {
                    tempCode = tempCode,
                    message = msgError,
                    status = submitResult > 0 && string.IsNullOrEmpty(msgError) ? "Success" : "Error",
                    isSaved = submitResult > 0
                };

                if (submitResult <= 0 && string.IsNullOrEmpty(msgError))
                {
                    response.message = "Submit upload failed.";
                }

                new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
                return response;
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);

                return new ExcelResponse
                {
                    tempCode = tempCode,
                    message = ex.Message,
                    status = "Error",
                    isSaved = false
                };
            }
        }
        public List<ProgramMaterialUploadResponse> GetMaterialsUploadValidationByTempCode(string tempCode)
        {
            string methodName = "GetMaterialsUploadValidationByTempCode";
            var result = new List<ProgramMaterialUploadResponse>();
            try
            {
                result = Db.ProgramRepository.GetUploadValidationByTempCode(tempCode);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public List<ProgramDetail> GetProgramDetailListByProgramId(int programId)
        {
            string methodName = "GetProgramDetailListByProgramId";
            var result = new List<ProgramDetail>();
            try
            {
                result = Db.ProgramRepository.GetProgramDetailListByProgramId(programId);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
    }
}
