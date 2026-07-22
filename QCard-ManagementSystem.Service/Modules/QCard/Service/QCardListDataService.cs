using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ExcelDataReader;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Domain.Modules.QCard.Model;
using QCard.ManagementSystem.Domain.Modules.QCard.Request;
using QCard.ManagementSystem.Domain.Modules.QCard.ViewModel;
using QCard.ManagementSystem.Infrastructure.Helper;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Service.Modules.Core.Service;
using QCard.ManagementSystem.Service.Modules.QCard.Validator;

namespace QCard.ManagementSystem.Service.Modules.QCard.Service
{
    public class QCardListDataService : DbService
    {
        private readonly long _userId;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailService _emailService;

        public QCardListDataService(IDbHelper db, IEmailService emailService, IWebHostEnvironment env) : base(db)
        {
            _userId = SessionManager.Id;
            _env = env;
            _emailService = emailService;
        }

        public DataSourceResult GetDataSourceResult(DataSourceRequest request)
        {
            string methodName = "GetDataSourceResult";
            var result = new DataSourceResult();

            try
            {
                if (request.Filters != null && request.Filters.Count > 0)
                {
                    foreach (var filter in request.Filters)
                    {
                        if (filter is FilterDescriptor descriptor)
                        {
                            if (descriptor.Member != null && descriptor.Member.Equals("CARD_NO", StringComparison.OrdinalIgnoreCase))
                            {
                                if (descriptor.Value is string strValue)
                                {
                                    descriptor.Value = strValue.Replace("-", "");
                                }
                            }
                        }
                        else if (filter is CompositeFilterDescriptor composite)
                        {
                            foreach (var subFilter in composite.FilterDescriptors)
                            {
                                if (subFilter is FilterDescriptor subDescriptor)
                                {
                                    if (subDescriptor.Member != null && subDescriptor.Member.Equals("CARD_NO", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (subDescriptor.Value is string strValue)
                                        {
                                            subDescriptor.Value = strValue.Replace("-", "");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                result = Db.QCardListRepository.GetDataSourceResult(request);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
                result = new DataSourceResult();
            }

            return result;
        }
        public List<long> GetAllQCardIds()
        {
            var result = Db.QCardListRepository.GetAllQCardIds();
            return result;
        }
        public BasicResponse Save(QCardRequest qcard)
        {
            string methodName = "Save";
            BasicResponse message = new();

            try
            {
                QCardValidator dataValidator = new();
                var validatorResult = dataValidator.Validate(qcard);

                if (!validatorResult.IsValid)
                {
                    message.Validate = false;
                    message.Success = false;
                    message.Message = JsonSerializer.Serialize(validatorResult.Errors);
                    return message;
                }

                var result = Db.QCardListRepository.Save(qcard, _userId);

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
        public BasicDataResponse GetQCardDetail(int id)
        {
            string methodName = "GetQCardDetail";
            var result = new BasicDataResponse();
            try
            {
                QCardModel? qcardDetail = Db.QCardListRepository.GetQCardDetail(id);

                result.Data = qcardDetail;
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

        public BasicResponse SaveTopup(TopupRequest topup)
        {
            string methodName = "Save";
            BasicResponse message = new();

            try
            {
                // ============= VALIDASI =============
                TopupValidator dataValidator = new();
                var validatorResult = dataValidator.Validate(topup);

                if (!validatorResult.IsValid)
                {
                    message.Validate = false;
                    message.Success = false;
                    message.Message = JsonSerializer.Serialize(validatorResult.Errors);
                    return message;
                }

                // ============= SAVE TOPUP =============
                var result = Db.QCardListRepository.SaveTopup(topup, _userId);

                if (result <= 0)
                    throw new Exception("Failed to save data");

                // ==========================================================
                //      AMBIL APPROVAL PER SERIAL NUMBER DALAM DETAILS
                // ==========================================================
                foreach (var detail in topup.DETAILS)
                {
                    var topupInfo = Db.QCardListRepository.GetTopupApprovalInfo(
                        _userId,
                        detail.SERIAL_NO
                    )?.FirstOrDefault();

                    if (topupInfo == null)
                    {
                        new ActivityLogService(Db).CreateErrorLog(
                            methodName,
                            this.GetType().Name,
                            $"Topup approval info not found for Serial={detail.SERIAL_NO}, Card={topup.CARD_ID}",
                            null
                        );
                        continue;
                    }

                    int approvalLevelId = topupInfo.NEXT_APPROVER_ROLE_ID;

                    var emailList = Db.QCardListRepository.GetEmailListByApprovalLevelAsync(approvalLevelId);
                    var approvalName = emailList.Select(x => x.APPROVAL_NAME).FirstOrDefault() ?? string.Empty;

                    _ = _emailService.SendBulkTaskApprovalNotificationAsync(
                        emailList,
                        topupInfo.SERIAL_NO,
                        approvalName,
                        _userId,
                        topupInfo.CARD_NO,
                        topupInfo.CATEGORY_PROGRAM,
                        topupInfo.COMPANY_NAME
                    );
                }

                // ============= SUCCESS RESPONSE =============
                message.Success = true;
                message.Validate = true;
                message.Message = string.Empty;

                new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db)
                    .CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);

                message.Success = false;
                message.Validate = true;
                message.Message = ex.Message;
            }

            return message;
        }


        public BasicDataResponse CheckQCardByCardNo(string qcardNo)
        {
            string methodName = "CheckQCardByCardNo";
            var result = new BasicDataResponse();
            try
            {
                QCardModel? qcard = Db.QCardListRepository.CheckQCardByCardNo(qcardNo).FirstOrDefault();

                if (qcard != null)
                {
                    result.Data = qcard;
                    result.Success = true;
                    result.Message = string.Empty;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Q-Card not found. Please verify your Q-Card number and try again.";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Terjadi kesalahan sistem. " + ex.Message;
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
                var result = Db.QCardListRepository.Delete(id);

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

        public QCardListView GetQCardById(string id)
        {
            var empty = string.Empty;
            return Db.QCardListRepository.ListQCard("GetList", id, empty).FirstOrDefault();
        }
        public BasicDataResponse GetSelectedQCardPrint(string json)
        {
            var result = new BasicDataResponse();
            try
            {
                var qCards = Db.QCardListRepository.GetSelectedQCardPrint(json);
                var webroot = _env.WebRootPath;
                qCards.frontSVGPath = System.IO.Path.Combine(
                    _env.WebRootPath,
                    "img", "QCardImage", "QCard_Front_2.svg");
                qCards.backSVGPath = System.IO.Path.Combine(
                    _env.WebRootPath,
                    "img", "QCardImage", "QCard_Back.svg");
                result.Data = qCards;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Terjadi kesalahan sistem. " + ex.Message;
            }
            return result;
        }

        public ExcelResponse HandleExcelUpload(IEnumerable<IFormFile> upload)
        {
            const string methodName = "HandleExcelUpload";
            var serialNo = new List<SerialNoUpload>();
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
                                                       reader.GetValue(1).ToString() == "Serial No." &&
                                                       reader.GetValue(2).ToString() == "Product Group" &&
                                                       reader.GetValue(3).ToString() == "Product Model") ? true : false;
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
                            var itemSERIAL_NO = reader.GetValue(1)?.ToString()?.Trim();
                            var itemPRODUCT_GROUP = reader.GetValue(2)?.ToString()?.Trim();
                            var itemPRODUCT_MODEL = reader.GetValue(3)?.ToString()?.Trim();

                            // 🔹 Validasi Part No
                            if (string.IsNullOrWhiteSpace(itemSERIAL_NO))
                                itemMessage += "Serial No. is required, ";

                            // Tambahkan ke list
                            serialNo.Add(new SerialNoUpload
                            {
                                SERIAL_NO = itemSERIAL_NO,
                                PRODUCT_GROUP = itemPRODUCT_GROUP,
                                PRODUCT_MODEL = itemPRODUCT_MODEL,
                                ORDER_INDEX = serialNo.Count + 1,
                                MESSAGE = itemMessage.TrimEnd(' ', ','), // simpan pesan error jika ada
                                STATUS = string.IsNullOrEmpty(itemMessage) ? "Success" : "Error"
                            });
                        }
                    }
                }

                if (serialNo.Count == 0)
                    throw new Exception("Your File has no data");

                var result = UploadProcess(serialNo, out string tempCode, out string msgError);

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
        public DataSourceResult GetUploadSerialNoTemporary(DataSourceRequest request, string tempCode)
        {
            string methodName = "GetUploadSerialNoTemporary";
            var result = new DataSourceResult();
            try
            {
                result = Db.QCardListRepository.GetUploadSerialNoValidation(request, tempCode);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public bool UploadProcess(List<SerialNoUpload> data, out string tempcode, out string msgerror)
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

                var uploadResult = Db.QCardListRepository.ValidateUploadProcess(data, by, out tempcode, out msgerror);

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
        public BasicDataResponse GetUploadedSerialNo(string tempCode)
        {
            string methodName = "GetUploadedSerialNo";
            var result = new BasicDataResponse();
            try
            {
                List<UploadedSerialNo?> uploadedMaterials = Db.QCardListRepository.GetUploadedSerialNo(tempCode);

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
        public DataSourceResult GetSerialNoQCard(DataSourceRequest request, int cardId)
        {
            string methodName = "GetSerialNoQCard";
            var result = new DataSourceResult();
            try
            {
                result = Db.QCardListRepository.GetSerialNoQCard(request, cardId);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public ExcelResponse SubmitExcelUpload(string tempCode)
        {
            const string methodName = "SubmitExcelUpload";

            try
            {
                string currentUser = SessionManager.Current;

                var submitResult = Db.QCardListRepository.SubmitFinalUpload(tempCode, currentUser, out string msgError);

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
        public List<SerialNoUploadResponse> GetSerialNoUploadValidationByTempCode(string tempCode)
        {
            string methodName = "GetSerialNoUploadValidationByTempCode";
            var result = new List<SerialNoUploadResponse>();
            try
            {
                result = Db.QCardListRepository.GetUploadValidationByTempCode(tempCode);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public bool isSQLInjectString(string strWords)
        {
            strWords = Regex.Replace(strWords, @"[^\u0000-\u007F]+", string.Empty);

            bool isSQLInjection = false;
            List<string> badChars = new List<string>() { "select", "drop", ";", "--", "insert", "delete", "xp_", "¿", "waitfor delay" };

            for (int i = 0; i < badChars.Count; i++)
            {
                if (strWords.Contains(badChars[i]))
                {
                    isSQLInjection = true;
                    break;
                }
            }

            return isSQLInjection;
        }

        private void RecurseFilterDescriptors(IList<IFilterDescriptor> requestFilters, List<FilterDescriptor> allFilterDescriptors)
        {
            foreach (var filterDescriptor in requestFilters)
            {
                if (filterDescriptor is FilterDescriptor)
                {
                    allFilterDescriptors.Add((FilterDescriptor)filterDescriptor);
                }
                else if (filterDescriptor is CompositeFilterDescriptor composite)
                {
                    RecurseFilterDescriptors(composite.FilterDescriptors, allFilterDescriptors);
                }
            }
        }
        public string GetPdfOrientation()
        {
            return Db.QCardListRepository.GetPdfOrientationFromDb() ?? "Portrait";
        }
    }

    public class ModelException : Exception
    {
        public string Title { get; }

        public ModelException(string title, string message)
            : base(message)
        {
            Title = title;
            this.Data.Add("Title", title);
        }
    }
}
