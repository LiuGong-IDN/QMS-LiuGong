using ClosedXML.Excel;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Hosting;
using Modules.MyQCard.Model;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Domain.Modules.MyQCard.Request;
using QCard.ManagementSystem.Infrastructure.Helper;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Service.Modules.Core.Service;

namespace QCard.ManagementSystem.Service.Modules.MyQCard.Service
{
    public class ClaimHistoryService(IDbHelper db, IEmailService emailService, IWebHostEnvironment env) : DbService(db)
    {
        private readonly IEmailService _emailService = emailService;
        private readonly long _userId = SessionManager.Id;
        private readonly long _UserCompanyId = SessionManager.CompanyId;


        public DataSourceResult GetDataSourceResult(DataSourceRequest request, string claimNo, string serialNo, int status)
        {
            string methodName = "GetDataSourceResult";
            var result = new DataSourceResult();
            try
            {
                result = Db.ClaimHistoryRepository.GetDataSourceResult(request, _userId, claimNo, serialNo, status);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicDataResponse GetClaimProgramDetail(int id)
        {
            string methodName = "GetClaimProgramDetail";
            var result = new BasicDataResponse();
            try
            {
                IEnumerable<ClaimDetail> claimDetail = Db.ClaimHistoryRepository.GetClaimProgramDetail(_userId, id);

                result.Data = claimDetail;
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
        public BasicDataResponse GetClaimVoucherDetail(int id)
        {
            string methodName = "GetClaimVoucherDetail";
            var result = new BasicDataResponse();
            try
            {
                IEnumerable<ClaimDetail> claimDetail = Db.ClaimHistoryRepository.GetClaimVoucherDetail(_userId, id);

                result.Data = claimDetail;
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
        public DataSourceResult GetClaimHistoryProgramDetail(DataSourceRequest request, int claimId)
        {
            string methodName = "GetClaimedProgram";
            var result = new DataSourceResult();
            try
            {
                result = Db.ClaimHistoryRepository.GetClaimHistoryProgramDetail(request, claimId);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public DataSourceResult GetClaimHistoryVoucherDetail(DataSourceRequest request, int claimId)
        {
            string methodName = "GetClaimedProgram";
            var result = new DataSourceResult();
            try
            {
                result = Db.ClaimHistoryRepository.GetClaimHistoryVoucherDetail(request, claimId);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        
        public List<ClaimProgramHistoryDetail> GetProgramDetailHistory(int claimId, int claimDetailId, int programId)
        {
            string methodName = "GetProgramDetailHistory";
            var result = new List<ClaimProgramHistoryDetail>();
            try
            {
                result = Db.ClaimHistoryRepository.GetProgramDetailHistory(claimId, claimDetailId, programId);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicResponse UpdateClaim(UpdateClaimRequest request)
        {
            string methodName = "UpdateClaim";

            BasicResponse message = new();
            try
            {
                var result = Db.ClaimHistoryRepository.UpdateClaim(request, _UserCompanyId, _userId);
                var categoryProgarm = new ConfigService(Db).GetConfigByIdAndValueText("CLAIM_TYPE", "Program").SYSTEM_TYPE;


                if (result > 0)
                {
                    var listclaim = Db.ClaimHistoryRepository.GetMachineClaimInfo(
                        _userId,
                        request.SERIAL_NO,
                        request.CLAIM_NO,
                        categoryProgarm,
                        request.CLAIM_ID
                    );

                    var claimData = listclaim.FirstOrDefault();
                    if (claimData == null)
                    {
                        throw new Exception($"❌ Claim tidak ditemukan! User={_userId}, Claim No={request.CLAIM_NO}");
                    }

                    string companyName = claimData.COMPANY_NAME ?? string.Empty;
                    int pdcId = claimData.PDC;
                    var emailList = Db.ClaimHistoryRepository.GetEmailListByPdcAsync(pdcId);
                    var pdcName = emailList.Select(x => x.PDC_NAME).FirstOrDefault() ?? string.Empty;

                    _ = _emailService.SendBulkClaimUpdateNotificationAsync(
                        emailList,
                        claimData.SERIAL_NO,
                        pdcName,
                        _userId,
                        claimData.CLAIM_TYPE_NAME ?? "Program",
                        claimData.CLAIM_NO,
                        companyName
                    );


                    message.Success = true;
                    message.Message = string.Empty;
                }
                else
                {
                    throw new Exception("Failed Update Data");
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
        public BasicResponse UpdateClaimVoucher(UpdateClaimVoucherRequest request)
        {
            string methodName = "UpdateClaimVoucherRequest";
            BasicResponse message = new();

            try
            {
                var result = Db.ClaimHistoryRepository.UpdateClaimVoucher(request, _UserCompanyId, _userId);
                var categoryVoucher = new ConfigService(Db).GetConfigByIdAndValueText("CLAIM_TYPE", "Voucher").SYSTEM_TYPE;

                if (result > 0)
                {
                    var listclaim = Db.ClaimHistoryRepository.GetMachineClaimInfo(
                        _userId,
                        request.SERIAL_NO,
                        request.CLAIM_NO,
                        categoryVoucher,
                        request.CLAIM_ID
                    );

                    var claimData = listclaim.FirstOrDefault();
                    if (claimData == null)
                    {
                        throw new Exception($"❌ Claim tidak ditemukan setelah save! User={_userId}, Claim No={request.CLAIM_NO}");
                    }

                    string companyName = claimData.COMPANY_NAME ?? string.Empty;

                    int pdcId = claimData.PDC; //kaya gini
                    var emailList = Db.ClaimHistoryRepository.GetEmailListByPdcAsync(pdcId);

                    var pdcName = emailList.Select(x => x.PDC_NAME).FirstOrDefault() ?? string.Empty;

                    _ = _emailService.SendBulkClaimUpdateNotificationAsync(
                        emailList,
                        claimData.SERIAL_NO,
                        pdcName,
                        _userId,
                        claimData.CLAIM_TYPE_NAME ?? "Voucher",
                        claimData.CLAIM_NO,
                        companyName
                    );

                    message.Success = true;
                    message.Message = string.Empty;
                }
                else
                {
                    throw new Exception("Failed Update Data");
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
        public BasicResponse ConfirmPartialPickup(string claimNo)
        {
            string methodName = "ConfirmPartialPickup";
            BasicResponse message = new();
            try
            {
                var result = Db.ClaimHistoryRepository.ConfirmPartialPickup(claimNo, _userId);

                if (result > 0)
                {
                    message.Success = true;
                    message.Message = string.Empty;
                }
                else
                {
                    throw new Exception("Failed Update Data");
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
        public byte[] ExportClaimToExcel(string type)
        {
            var userId = SessionManager.Id;
            var pdcId = SessionManager.PdcId;
            var data = Db.ClaimHistoryRepository.GetDataDownload(type, pdcId, userId);

            var templateFileName = type switch
            {
                "List" => "ClaimListTemplate.xlsx",
                "All" => "ClaimAllTemplate.xlsx",
                "PendingParts" => "ClaimPendingPartsTemplate.xlsx",
                _ => "ClaimListTemplate.xlsx"
            };

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", templateFileName);
            if (!File.Exists(templatePath))
                throw new FileNotFoundException("Template file not found.", templatePath);

            using var workbook = new XLWorkbook(templatePath);
            var sheet = workbook.Worksheet(1);
            int currentRow = 2;

            if (data != null && data.Any())
            {
                foreach (var item in data)
                {
                    int col = 1;

                    string S(string? v) => v ?? "Not found";
                    int I(int? v) => v ?? 0;
                    string D(DateTime? v) => v?.ToString("dd-MMM-yyyy") ?? string.Empty;

                    // Kolom umum (semua tipe)
                    sheet.Cell(currentRow, col++).Value = S(item.CLAIM_NO);
                    sheet.Cell(currentRow, col++).Value = S(item.SERIAL_NO);
                    sheet.Cell(currentRow, col++).Value = S(item.ASSET_TYPE_NAME);
                    sheet.Cell(currentRow, col++).Value = S(item.MODEL_NAME);
                    sheet.Cell(currentRow, col++).Value = D(item.CLAIM_DATE);
                    sheet.Cell(currentRow, col++).Value = S(item.PDC_NAME);
                    sheet.Cell(currentRow, col++).Value = S(item.STATUS_NAME);

                    if (type == "All" || type == "PendingParts")
                    {
                        sheet.Cell(currentRow, col++).Value = S(item.PROGRAM_CODE);
                        sheet.Cell(currentRow, col++).Value = S(item.PROGRAM_NAME);
                        sheet.Cell(currentRow, col++).Value = I(item.PACKAGE_QTY);
                        sheet.Cell(currentRow, col++).Value = I(item.CLAIM_QTY);
                        sheet.Cell(currentRow, col++).Value = S(item.PART_NO);
                        sheet.Cell(currentRow, col++).Value = S(item.PART_NAME);
                        sheet.Cell(currentRow, col++).Value = D(item.PICKED_UP_DATE);
                        sheet.Cell(currentRow, col++).Value = S(item.DELIVERY_NOTE_NO);
                        sheet.Cell(currentRow, col++).Value = S(item.RESI);
                        sheet.Cell(currentRow, col++).Value = I(item.PACKAGE_PART_QTY);
                        sheet.Cell(currentRow, col++).Value = I(item.QTY);
                        sheet.Cell(currentRow, col++).Value = I(item.AVAILABILITY_QTY);
                        sheet.Cell(currentRow, col++).Value = I(item.PENDING_QTY);
                        sheet.Cell(currentRow, col++).Value = I(item.CLAIMED_QTY);
                    }


                    // Terapkan border
                    sheet.Range(currentRow, 1, currentRow, col - 1)
                        .Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                        .Border.SetInsideBorder(XLBorderStyleValues.Thin);

                    currentRow++;
                }
            }

            sheet.ColumnsUsed().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
