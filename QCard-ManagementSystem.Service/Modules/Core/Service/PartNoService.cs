using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ExcelDataReader;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Infrastructure.Helper;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Service.Modules.Core.Validator;


namespace QCard.ManagementSystem.Service.Modules.Core.Service
{
    public class PartNoService : DbService
    {
        private readonly IEmailService _emailService;
        private readonly TimeSpan TokenTtl = TimeSpan.FromMinutes(30);
        private readonly IWebHostEnvironment _env;
        private readonly long _userId = SessionManager.Id;
        public PartNoService(IDbHelper db, IEmailService emailService, IWebHostEnvironment env) : base(db)
        {
            _emailService = emailService;
             _env = env;
        }

        public DataSourceResult GetDataSourceResult(DataSourceRequest request, string partNo)
        {
            string methodName = "GetDataSourceResult";
            var result = new DataSourceResult();
            try
            {
                result = Db.PartNoRepository.GetDataSourceResult(request, partNo);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicResponse Save(PartNoRequest partno)
        {
            string methodName = "Save";
            BasicResponse message = new();

            try
            {
                PartNoValidator dataValidator = new();
                var validatorResult = dataValidator.Validate(partno);

                if (!validatorResult.IsValid)
                {
                    message.Validate = false;
                    message.Success = false;
                    message.Message = JsonSerializer.Serialize(validatorResult.Errors);
                    return message;
                }

                var result = Db.PartNoRepository.Save(partno, _userId);

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
        public ExcelResponse HandleExcelUpload(IEnumerable<IFormFile> upload)
        {
            const string methodName = "HandleExcelUpload";
            var partNos = new List<PartNoUpload>();
            ExcelResponse response;

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
                        int step = 10;

                        while (reader.Read())
                        {
                            string itemMessage = "";
                            pos++;

                            if (reader.Depth == 0 || pos <= step || reader.GetValue(0) == null)
                            {
                                if (pos == step)
                                {
                                    if (reader.FieldCount != 5)
                                        throw new Exception("Format Excel Invalid");

                                    var titleMerge = string.Concat(reader.GetValue(0)?.ToString(), "|", reader.GetValue(1)?.ToString(), "|", reader.GetValue(2)?.ToString(), "|", reader.GetValue(3)?.ToString(), "|", reader.GetValue(4)?.ToString());

                                    if (!titleMerge.Trim().Equals("Action|No|Part No.|Part Name|Retail Price"))
                                        throw new Exception("Format Excel Invalid");
                                }

                                continue;
                            }

                            if (!string.IsNullOrEmpty(reader.GetValue(0)?.ToString()))
                            {
                                var itemACTION = reader.GetValue(0)?.ToString() ?? string.Empty;
                                var itemPART_NO = reader.GetValue(2)?.ToString() ?? string.Empty;
                                var itemPART_NAME = reader.GetValue(3)?.ToString() ?? string.Empty;
                                //var itemRETAIL_PRICE = reader.GetValue(4);
                                object rawPrice = reader.GetValue(4);
                                int priceValue = 0;

                                if (rawPrice == null || string.IsNullOrWhiteSpace(rawPrice.ToString()))
                                {
                                    itemMessage = "Retail Price is required, ";
                                }
                                else if (rawPrice is double d)
                                {
                                    priceValue = Convert.ToInt32(d);
                                }
                                else if (rawPrice is int i)
                                {
                                    priceValue = i;
                                }
                                else
                                {
                                    var cleaned = rawPrice.ToString().Trim().Replace(".", "").Replace(",", "");
                                    if (!int.TryParse(cleaned, out priceValue))
                                    {
                                        itemMessage = "Invalid Retail Price format, ";
                                    }
                                }

                                if (string.IsNullOrWhiteSpace(itemACTION))
                                    itemMessage = "ACTION is required, ";
                                else if (string.IsNullOrWhiteSpace(itemPART_NO))
                                    itemMessage = "Part No. is required, ";
                                else if (string.IsNullOrWhiteSpace(itemPART_NAME))
                                    itemMessage = "Part Name is required, ";
                                else if (partNos.Any(x => x.PART_NO == itemPART_NO))
                                    itemMessage = "Duplicate Part No., ";

                                partNos.Add(new PartNoUpload
                                {
                                    PART_NO = itemPART_NO,
                                    PART_NAME = itemPART_NAME,
                                    RETAIL_PRICE = priceValue,
                                    MESSAGE = itemMessage,
                                    STATUS = string.IsNullOrEmpty(itemMessage) ? "Success" : "Error",
                                    NO = pos - step,
                                    ACTION = itemACTION.ToUpper()
                                });
                            }
                        }
                    }
                }

                if (partNos.Count == 0)
                    throw new Exception("Your File has no data");

                var result = UploadProcess(partNos, out string tempCode, out string msgError);

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
                    message = ex.Message,
                    status = "Error",
                    canSubmit = false,
                    isSaved = false
                };

                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }

            return response;
        }
        public DataSourceResult GetUploadPartNoTemporary(DataSourceRequest request, string tempCode)
        {
            string methodName = "GetUploadPartNoTemporary";
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

                result = Db.PartNoRepository.GetUploadPartNoTemporary(request);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public bool UploadProcess(List<PartNoUpload> data, out string tempcode, out string msgerror)
        {
            bool result;
            string methodName = "UploadProcess";
            tempcode = string.Empty;

            try
            {
                string currentUser = SessionManager.Current;

                var uploadResult = Db.PartNoRepository.ValidateUploadProcess(data, currentUser, out tempcode, out msgerror);

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

                var submitResult = Db.PartNoRepository.SubmitFinalUpload(tempCode, currentUser, out string msgError);

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
        public List<PartNoUploadResponse> GetPartNoUploadValidationByTempCode(string tempCode)
        {
            string methodName = "GetPartNoUploadValidationByTempCode";
            var result = new List<PartNoUploadResponse>();
            try
            {
                result = Db.PartNoRepository.GetPartNoUploadValidationByTempCode(tempCode);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicResponse Delete(string id)
        {
            string methodName = "Delete";
            BasicResponse message = new();
            try
            {
                var result = Db.PartNoRepository.Delete(id);

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
        
        public PartNoView GetPartNoById(int id, string partNo)
        {
            var empty = string.Empty;
            return Db.PartNoRepository.ListPart("GetList", id, partNo).FirstOrDefault();
        }

        public bool isSQLInjectString(string strWords)
        {
            strWords = Regex.Replace(strWords, @"[^\u0000-\u007F]+", string.Empty);

            bool isSQLInjection = false;
            List<string> badChars = new List<string>() { "select", "drop", ";", "--", "insert", "delete", "xp_", "¿", "waitfor delay" };

            for (int i = 0; i < badChars.Count(); i++)
            {
                if (strWords.Contains(badChars[i]))
                    isSQLInjection = true;
                break;
            }

            return isSQLInjection;
        }
    }
}
