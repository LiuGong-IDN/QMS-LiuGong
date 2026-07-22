using FluentValidation;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Hosting;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Domain.Modules.QCard.ViewModel;
using QCard.ManagementSystem.Infrastructure.Helper;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Service.Modules.Core.Validator;
using System.Text.Json;
using System.IO;
using System.Text;
using Kendo.Mvc;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;

namespace QCard.ManagementSystem.Service.Modules.Core.Service
{
    public class CustomerService : DbService
    {
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _env;
        private readonly long _userId = SessionManager.Id;

        public CustomerService(IDbHelper db, IEmailService emailService, IWebHostEnvironment env) : base(db)
        {
            _emailService = emailService;
            _env = env;
        }

        public DataSourceResult GetDataSourceResult(DataSourceRequest request, string companyCode, string companyName)
        {
            string methodName = "GetDataSourceResult";
            var result = new DataSourceResult();
            try
            {
                result = Db.CustomerRepository.GetDataSourceResult(request, companyCode, companyName);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }

        public BasicResponse Save(CustomerRequest customer)
        {
            string methodName = "Save";
            BasicResponse message = new();

            try
            {
                CustomerValidator dataValidator = new();
                var validatorResult = dataValidator.Validate(customer);

                if (!validatorResult.IsValid)
                {
                    message.Validate = false;
                    message.Success = false;
                    message.Message = JsonSerializer.Serialize(validatorResult.Errors);
                    return message;
                }

                var result = Db.CustomerRepository.Save(customer, _userId);

                if (result > 0)
                {
                    message.Success = true;
                    message.Validate = true;
                    message.Message = string.Empty;

                    new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
                }
                else
                {
                    throw new Exception("Failed to save data");
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
                message.Success = false;
                message.Validate = true;
                message.Message = ex.Message;
            }

            return message;
        }

        public BasicResponse Update(CustomerRequest customer)
        {
            string methodName = "Update";
            BasicResponse message = new BasicResponse();

            try
            {
                // Validasi
                var validatorResult = new CustomerValidator().Validate(customer);
                if (!validatorResult.IsValid)
                {
                    message.Validate = false;
                    throw new Exception(JsonSerializer.Serialize(validatorResult.Errors));
                }

                var result = Db.CustomerRepository.Save(customer, _userId); 

                if (result > 0)
                {
                    message.Success = true;
                }
                else
                {
                    throw new Exception("Update failed. Company name may already exist or record not found.");
                }

                new ActivityLogService(Db).CreateSuccessLog(methodName, GetType().Name);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, GetType().Name, ex.Message, ex.StackTrace);
                message.Success = false;
                message.Message = ex.Message;
            }

            return message;
        }

        public BasicResponse Delete(string id)
        {
            string methodName = "Delete";
            BasicResponse message = new();
            try
            {
                var result = Db.CustomerRepository.Delete(id);

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

        public CustomerMaintenance GetById(long id)
        {
            string methodName = "GetById";
            try
            {
                return Db.CustomerRepository.GetById(id);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, GetType().Name, ex.Message, ex.StackTrace);
                throw;
            }
        }

        //PROCESS UPLOAD EXCEL
        public ExcelResponse HandleExcelUpload(IEnumerable<IFormFile> upload)
        {
            const string methodName = "HandleExcelUpload";
            var customers = new List<CustomerUpload>();
            ExcelResponse response;

            try
            {
                // ✅ Validation 1: Check if any files were uploaded
                if (upload == null || !upload.Any())
                {
                    throw new Exception("No file uploaded. Please select an .xlsx file first.");
                }

                // ✅ Validation 2: Check all file formats before processing
                var invalidFiles = upload
                    .Where(f => f != null) // Avoid NullReferenceException
                    .Where(f => !Path.GetExtension(f.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                    .Select(f => f.FileName)
                    .ToList();

                if (invalidFiles.Any())
                {
                    var fileList = string.Join(", ", invalidFiles);
                    throw new Exception($"The following files are not supported: {fileList}. File format must be .xlsx");
                }

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using (var stream = new MemoryStream())
                {
                    foreach (var file in upload)
                    {
                        file.CopyTo(stream);
                        stream.Position = 0;

                        using var reader = ExcelReaderFactory.CreateReader(stream);
                        int pos = 0;
                        int step = 10;

                        while (reader.Read())
                        {
                            string itemMessage = "";
                            pos++;

                            // Skip header/template rows
                            if (reader.Depth == 0 || pos <= step || reader.GetValue(0) == null)
                            {
                                if (pos == step)
                                {
                                    if (reader.FieldCount < 5)
                                        throw new Exception("Invalid Excel format");

                                    var titleMerge = string.Concat(
                                        reader.GetValue(0)?.ToString(), "|",
                                        reader.GetValue(1)?.ToString(), "|",
                                        reader.GetValue(2)?.ToString(), "|",
                                        reader.GetValue(3)?.ToString(), "|",
                                        reader.GetValue(4)?.ToString()
                                    );

                                    if (!titleMerge.Trim().Equals("Action|No|Company Code|Company Name|Address"))
                                        throw new Exception("Invalid Excel format");
                                }

                                continue;
                            }

                            // Process data rows (existing logic remains the same)
                            if (!string.IsNullOrEmpty(reader.GetValue(0)?.ToString()))
                            {
                                var itemACTION = reader.GetValue(0)?.ToString()?.Trim() ?? string.Empty;
                                var itemCOMPANY_CODE = reader.GetValue(2)?.ToString()?.Trim() ?? string.Empty;
                                var itemCOMPANY_NAME = reader.GetValue(3)?.ToString()?.Trim() ?? string.Empty;
                                var itemADDRESS = reader.GetValue(4)?.ToString()?.Trim() ?? string.Empty;

                                itemMessage = "";

                                if (string.IsNullOrWhiteSpace(itemACTION))
                                    itemMessage += "ACTION is required, ";

                                if (string.IsNullOrWhiteSpace(itemCOMPANY_CODE))
                                    itemMessage += "Company Code is required, ";

                                if (string.IsNullOrWhiteSpace(itemCOMPANY_NAME))
                                    itemMessage += "Company Name is required, ";

                                if (string.IsNullOrWhiteSpace(itemADDRESS))
                                    itemMessage += "Address is required, ";

                                if (string.IsNullOrEmpty(itemMessage))
                                {
                                    if (customers.Any(x => x.COMPANY_CODE == itemCOMPANY_CODE))
                                        itemMessage += "Duplicate Company Code, ";
                                }

                                customers.Add(new CustomerUpload
                                {
                                    COMPANY_CODE = itemCOMPANY_CODE,
                                    COMPANY_NAME = itemCOMPANY_NAME,
                                    ADDRESS = itemADDRESS,
                                    MESSAGE = itemMessage,
                                    STATUS = string.IsNullOrEmpty(itemMessage) ? "Success" : "Error",
                                    NO = pos - step,
                                    ACTION = itemACTION.ToUpper()
                                });
                            }
                        }
                    }
                }

                if (customers.Count == 0)
                    throw new Exception("Your file has no data");

                var result = UploadProcess(customers, out string tempCode, out string msgError);

                response = new ExcelResponse
                {
                    tempCode = tempCode,
                    message = msgError,
                    status = result && string.IsNullOrEmpty(msgError) ? "Success" : "Error",
                    canSubmit = result && string.IsNullOrEmpty(msgError),
                    isSaved = true
                };

                new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
            }
            catch (Exception ex)
            {
                response = new ExcelResponse
                {
                    tempCode = string.Empty,
                    message = ex.Message, // Clear error message will be displayed here
                    status = "Error",
                    canSubmit = false,
                    isSaved = false
                };

                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return response;
        }


        public DataSourceResult GetUploadCustomerTemporary(DataSourceRequest request, string tempCode)
        {
            string methodName = "GetUploadCustomerTemporary";
            var result = new DataSourceResult();
            try
            {
                if (!string.IsNullOrEmpty(tempCode))
                {
                    request.Filters = request.Filters ?? new List<IFilterDescriptor>();
                    request.Filters.Add(new FilterDescriptor
                    {
                        Member = "TEMPCODE",
                        Operator = FilterOperator.IsEqualTo,
                        Value = tempCode
                    });
                }

                result = Db.CustomerRepository.GetUploadCustomerTemporary(request);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }


        public bool UploadProcess(List<CustomerUpload> data, out string tempcode, out string msgerror)
        {
            bool result;
            string methodName = "UploadProcess";
            tempcode = string.Empty;

            try
            {

                var uploadResult = Db.CustomerRepository.ValidateUploadProcess(data, _userId, out tempcode, out msgerror);

                if (uploadResult > 0)
                {
                    result = true;
                }
                else
                {
                    throw new Exception(!string.IsNullOrEmpty(msgerror) ? msgerror : "Upload failed.");
                }

                new ActivityLogService(Db).CreateSuccessLog(methodName, GetType().Name);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, GetType().Name, ex.Message, ex.StackTrace);
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

                var submitResult = Db.CustomerRepository.SubmitFinalUpload(tempCode, _userId, out string msgError);

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

                new ActivityLogService(Db).CreateSuccessLog(methodName, GetType().Name);
                return response;
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, GetType().Name, ex.Message, ex.StackTrace);

                return new ExcelResponse
                {
                    tempCode = tempCode,
                    message = ex.Message,
                    status = "Error",
                    isSaved = false
                };
            }
        }

        public List<CustomerUploadResponse> GetCustomerUploadValidationByTempCode(string tempCode)
        {
            string methodName = "GetCustomerUploadValidationByTempCode";
            var result = new List<CustomerUploadResponse>();
            try
            {
                result = Db.CustomerRepository.GetCustomerUploadValidationByTempCode(tempCode);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }



    }
}
