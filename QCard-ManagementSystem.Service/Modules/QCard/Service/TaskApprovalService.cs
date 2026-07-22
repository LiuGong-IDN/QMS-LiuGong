using System.Text.Json;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Modules.QCard.Model;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Domain.Modules.QCard.Model;
using QCard.ManagementSystem.Domain.Modules.QCard.Request;
using QCard.ManagementSystem.Infrastructure.Helper;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Service.Modules.Core.Service;
using QCard.ManagementSystem.Service.Modules.QCard.Validator;

namespace QCard.ManagementSystem.Service.Modules.QCard.Service
{
    public class TaskApprovalService : DbService
    {
        private readonly long _userId;
        private readonly IEmailService _emailService;

        public TaskApprovalService(IDbHelper db, IEmailService emailService) : base(db)
        {
            _userId = SessionManager.Id;
            _emailService = emailService;
        }

        public DataSourceResult GetDataSourceResult(DataSourceRequest request, string cardNo, int customer)
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

                result = Db.TaskApprovalRepository.GetDataSourceResult(request, cardNo, customer, _userId);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
                result = new DataSourceResult();
            }

            return result;
        }
        public List<ApprovalList> GetAllTaskApprovalIds()
        {
            var result = Db.TaskApprovalRepository.GetAllTaskApprovalIds(_userId);
            return result;
        }

        public ApprovalList GetTaskApprovalById(int id)
        {
            var empty = string.Empty;
            return Db.TaskApprovalRepository.ListApproval("GetList", id, empty).FirstOrDefault();
        }
        public BasicDataResponse GetTopupDetail(int id)
        {
            string methodName = "GetTopupDetail";
            var result = new BasicDataResponse();
            try
            {
                ApprovalDetail? qcardDetail = Db.TaskApprovalRepository.GetTopupDetail(id);

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
        public BasicDataResponse GetApprovalHistoryDetail(long approvalHistoryId)
        {
            string methodName = "GetApprovalHistoryDetail";
            var result = new BasicDataResponse();
            try
            {
                ApprovalHistoryDetail? qcardDetail = Db.TaskApprovalRepository.GetApprovalHistoryDetail(approvalHistoryId);

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
        public DataSourceResult GetApprovalHistory(DataSourceRequest request, int documentId)
        {
            string methodName = "GetApprovalHistory";
            var result = new DataSourceResult();
            try
            {
                result = Db.TaskApprovalRepository.GetApprovalHistory(request, documentId);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public byte[] ExportTaskApprovalToExcel()
        {
            var data = Db.TaskApprovalRepository.GetDataDownload();

            string templatePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Areas", "Core", "template",
                "TaskApprovalAllTemplate.xlsx"
            );

            if (!System.IO.File.Exists(templatePath))
                throw new FileNotFoundException("Template file not found.", templatePath);

            using var workbook = new XLWorkbook(templatePath);
            var sheet = workbook.Worksheet(1);

            int headerRow = 1;
            int startRow = 2;
            int currentRow = startRow;

            // === Tuliskan header secara manual di baris pertama ===
            sheet.Cell(headerRow, 1).Value = "Q-Card No.";
            sheet.Cell(headerRow, 2).Value = "Serial No.";
            sheet.Cell(headerRow, 3).Value = "Category Claim";
            sheet.Cell(headerRow, 4).Value = "Customer";
            sheet.Cell(headerRow, 5).Value = "Top Up Date";
            sheet.Cell(headerRow, 6).Value = "Valid Thru";
            sheet.Cell(headerRow, 7).Value = "Product Group";
            sheet.Cell(headerRow, 8).Value = "Product Model";
            sheet.Cell(headerRow, 9).Value = "Purchase Date";
            sheet.Cell(headerRow, 10).Value = "Notes (Sugi Raharjo)";
            sheet.Cell(headerRow, 11).Value = "Evidence (Sugi Raharjo)";
            sheet.Cell(headerRow, 12).Value = "Notes (Sahdan Sanjaya)";
            sheet.Cell(headerRow, 13).Value = "Evidence (Sahdan Sanjaya)";
            sheet.Cell(headerRow, 14).Value = "Notes (COO Liugong)";
            sheet.Cell(headerRow, 15).Value = "Evidence (COO Liugong)";

            // === Isi data ===
            foreach (var item in data)
            {
                sheet.Cell(currentRow, 1).Value = item.CARD_NO;
                sheet.Cell(currentRow, 2).Value = item.SERIAL_NO;
                sheet.Cell(currentRow, 3).Value = item.CATEGORY_PROGRAM;
                sheet.Cell(currentRow, 4).Value = item.COMPANY_NAME;
                sheet.Cell(currentRow, 5).Value = item.TOPUP_DATE.ToString("dd-MMM-yyyy");
                sheet.Cell(currentRow, 6).Value = item.VALID_THRU;
                sheet.Cell(currentRow, 7).Value = item.PRODUCT_GROUP;
                sheet.Cell(currentRow, 8).Value = item.PRODUCT_MODEL;
                sheet.Cell(currentRow, 9).Value = item.PURCHASE_DATE.ToString("dd-MMM-yyyy");
                sheet.Cell(currentRow, 10).Value = item.NOTES_1;
                sheet.Cell(currentRow, 11).Value = item.EVIDENCE_1;
                sheet.Cell(currentRow, 12).Value = item.NOTES_2;
                sheet.Cell(currentRow, 13).Value = item.EVIDENCE_2;
                sheet.Cell(currentRow, 14).Value = item.NOTES_3;
                sheet.Cell(currentRow, 15).Value = item.EVIDENCE_3;

                // Tambahkan border tiap baris data
                sheet.Range(currentRow, 1, currentRow, 15)
                     .Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                     .Border.SetInsideBorder(XLBorderStyleValues.Thin);

                currentRow++;
            }

            // Tambahkan border untuk header juga
            sheet.Range(headerRow, 1, headerRow, 14)
                 .Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                 .Border.SetInsideBorder(XLBorderStyleValues.Thin)
                 .Font.SetBold(true);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public BasicResponse Save(ApprovalRequest approve)
        {
            string methodName = "Save";
            BasicResponse message = new();

            try
            {
                ApprovalValidator dataValidator = new();
                var validatorResult = dataValidator.Validate(approve);

                if (!validatorResult.IsValid)
                {
                    message.Validate = false;
                    message.Success = false;
                    message.Message = JsonSerializer.Serialize(validatorResult.Errors);
                    return message;
                }

                var result = Db.TaskApprovalRepository.Save(approve, _userId);

                if (result > 0)
                {
                    foreach (var docIdString in approve.DOCUMENT_APPROVAL_IDS)
                    {
                        // Pecah jika berisi banyak ID dalam satu string
                        var docIds = docIdString
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .ToList();

                        foreach (var docId in docIds)
                        {
                            var listtopup = Db.TaskApprovalRepository.GetTopupApprovalInfo(
                                _userId,
                                docId // <-- tetap string, tanpa convert
                            );

                            var topupData = listtopup.FirstOrDefault();
                            if (topupData == null)
                            {
                                throw new Exception($"❌ Topup tidak ditemukan! DocumentID={docId}, User={_userId}");
                            }
                            if (topupData.STATUS_ID == 20010 || topupData.STATUS_ID == 20014)
                            {
                                continue; // skip
                            }

                            int approvalLevelId = topupData.NEXT_APPROVER_ROLE_ID;

                            var emailList = Db.TaskApprovalRepository.GetEmailListByApprovalLevelAsync(approvalLevelId);

                            var approvalName = emailList
                                .Select(x => x.APPROVAL_NAME)
                                .FirstOrDefault() ?? string.Empty;

                            _ = _emailService.SendBulkTaskApprovalNotificationAsync(
                                emailList,
                                topupData.SERIAL_NO,
                                approvalName,
                                _userId,
                                topupData.CARD_NO,
                                topupData.CATEGORY_PROGRAM,
                                topupData.COMPANY_NAME
                            );
                        }
                    }

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
        public BasicResponse Reject(ApprovalRequest reject)
        {
            string methodName = "Reject";
            BasicResponse message = new();

            try
            {
                ApprovalValidator dataValidator = new();
                var validatorResult = dataValidator.Validate(reject);

                if (!validatorResult.IsValid)
                {
                    message.Validate = false;
                    message.Success = false;
                    message.Message = JsonSerializer.Serialize(validatorResult.Errors);
                    return message;
                }

                var result = Db.TaskApprovalRepository.Reject(reject, _userId);

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
    }
}
