using ClosedXML.Excel;
using FluentValidation;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Hosting;
using Modules.MyMachines.Model;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Domain.Modules.MyMachines.Model;
using QCard.ManagementSystem.Domain.Modules.MyMachines.Request;
using QCard.ManagementSystem.Infrastructure.Helper;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Service.Modules.MyMachince.Validator;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;


namespace QCard.ManagementSystem.Service.Modules.Core.Service
{
    public class MyMachinesService(IDbHelper db, IEmailService emailService, IWebHostEnvironment env) : DbService(db)
    {
        private readonly IEmailService _emailService = emailService;
        private readonly TimeSpan TokenTtl = TimeSpan.FromMinutes(30);
        private readonly IWebHostEnvironment _env = env;
        private readonly long _userId = SessionManager.Id;
        private readonly long _UserCompanyId = SessionManager.CompanyId;

        public DataSourceResult GetDataSourceResult(DataSourceRequest request, string serialNo, int model, int machineType)
        {
            string methodName = "GetDataSourceResult";
            var result = new DataSourceResult();
            var cardNo = SessionManager.CardNo ?? "0";
            try
            {
                result = Db.MyMachinesRepository.GetDataSourceResult(request, cardNo, serialNo, model, machineType);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }
        public BasicDataResponse GetMachinesDetail(int id)
        {
            string methodName = "GetMachinesDetail";
            var result = new BasicDataResponse();
            try
            {
                MachinesDetail? machinesDetail = Db.MyMachinesRepository.GetMachinesDetail(id);

                result.Data = machinesDetail;
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
        public BasicResponse Claim(SubmitClaimRequest request)
        {
            string methodName = "Save";
            BasicResponse message = new();
            try
            {
                //validator
                ClaimValidatorModel validatorModel = new()
                {
                    PDC_ID = request.PDC_ID
                };
                ClaimSubmitValidator dataValidator = new();
                var validatorResult = dataValidator.Validate(validatorModel);

                if (!validatorResult.IsValid)
                {
                    message.Validate = false;
                    throw new Exception(JsonSerializer.Serialize(validatorResult.Errors));
                }

                // kalau sudah oke tolong di hilangkan commentnya agar proses claim berjalan
                var result = Db.MyMachinesRepository.Claim(request, _UserCompanyId, _userId);
                var categoryProgarm = new ConfigService(Db).GetConfigByIdAndValueText("CLAIM_TYPE", "Program").SYSTEM_TYPE;

                if (result > 0)
                {
                    var listclaim = Db.ClaimHistoryRepository.GetMachineClaimInfo(
                        _userId,
                        request.SERIAL_NO,
                        request.CLAIM_NO,
                        categoryProgarm
                    );

                    var claimData = listclaim.FirstOrDefault();
                    if (claimData == null)
                    {
                        throw new Exception($"❌ Claim tidak ditemukan! User={_userId}, Claim No={request.CLAIM_NO}");
                    }

                    string companyName = claimData.COMPANY_NAME ?? string.Empty;
                    var emailList = Db.ClaimHistoryRepository.GetEmailListByPdcAsync(request.PDC_ID);
                    var pdcName = emailList.Select(x => x.PDC_NAME).FirstOrDefault() ?? string.Empty;

                    _ = _emailService.SendBulkMachineNotificationAsync(
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
        public BasicResponse ClaimVoucher(SubmitClaimVoucherRequest request)
        {
            string methodName = "Save";
            BasicResponse message = new();
            try
            {
                //validator
                ClaimValidatorModel validatorModel = new()
                {
                    PDC_ID = request.PDC_ID
                };
                ClaimSubmitValidator dataValidator = new();
                var validatorResult = dataValidator.Validate(validatorModel);

                if (!validatorResult.IsValid)
                {
                    message.Validate = false;
                    throw new Exception(JsonSerializer.Serialize(validatorResult.Errors));
                }

                // kalau sudah oke tolong di hilangkan commentnya agar proses claim berjalan
                var result = Db.MyMachinesRepository.ClaimVoucher(request, _UserCompanyId, _userId);
                var categoryVoucher = new ConfigService(Db).GetConfigByIdAndValueText("CLAIM_TYPE", "Voucher").SYSTEM_TYPE;

                if (result > 0)
                {
                    var listclaim = Db.ClaimHistoryRepository.GetMachineClaimInfo(
                        _userId,
                        request.SERIAL_NO,
                        request.CLAIM_NO,
                        categoryVoucher
                    );

                    var claimData = listclaim.FirstOrDefault();
                    if (claimData == null)
                    {
                        throw new Exception($"❌ Claim tidak ditemukan setelah save! User={_userId}, Claim No={request.CLAIM_NO}");
                    }

                    string companyName = claimData.COMPANY_NAME ?? string.Empty;
                    var emailList = Db.ClaimHistoryRepository.GetEmailListByPdcAsync(request.PDC_ID);
                    var pdcName = emailList.Select(x => x.PDC_NAME).FirstOrDefault() ?? string.Empty;

                    _ = _emailService.SendBulkMachineNotificationAsync(
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

        public byte[] ExportMyMachinesToExcel()
        {
            var cardNo = SessionManager.CardNo;
            var data = Db.MyMachinesRepository.GetDataDownload(cardNo);

            var templateFileName = "MyMachineAllTemplate.xlsx";

            var templatePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot", "templates",
                templateFileName
            );

            if (!File.Exists(templatePath))
                throw new FileNotFoundException("Template file not found.", templatePath);

            using var workbook = new XLWorkbook(templatePath);
            var sheet = workbook.Worksheet(1);

            int currentRow = 3;
            int mergeStartRow = currentRow;
            string lastSerial = null;

            int[] mergeColumns = { 1, 2, 3, 4, 5, 10, 11, 12 };

            foreach (var item in data)
            {
                string serial = item.SERIAL_NO;

                if (lastSerial != null && serial != lastSerial)
                {
                    if (mergeStartRow < currentRow - 1)
                    {
                        foreach (var col in mergeColumns)
                        {
                            sheet.Range(mergeStartRow, col, currentRow - 1, col).Merge();
                        }
                    }

                    mergeStartRow = currentRow;
                }

                sheet.Cell(currentRow, 1).Value = item.SERIAL_NO;
                sheet.Cell(currentRow, 2).Value = item.MACHINE_TYPE;
                sheet.Cell(currentRow, 3).Value = item.MODEL;
                sheet.Cell(currentRow, 4).Value = item.PURCHASE_DATE.ToString("dd-MMM-yyyy");
                sheet.Cell(currentRow, 5).Value = item.SERVICE_PARTY;

                // program (jangan merge)
                sheet.Cell(currentRow, 6).Value = item.PROGRAM_CODE;
                sheet.Cell(currentRow, 7).Value = item.PROGRAM_NAME;
                sheet.Cell(currentRow, 8).Value = item.TOTAL_PACKAGE_QTY;
                sheet.Cell(currentRow, 9).Value = item.TOTAL_CLAIMED_QTY;

                // balance (boleh merge)
                sheet.Cell(currentRow, 10).Value = item.INITIAL_BALANCE;
                sheet.Cell(currentRow, 10).Style.NumberFormat.Format = "\"Rp\" #,##0";

                sheet.Cell(currentRow, 11).Value = item.USED_BALANCE;
                sheet.Cell(currentRow, 11).Style.NumberFormat.Format = "\"Rp\" #,##0";

                sheet.Cell(currentRow, 12).Value = item.REMAINING_BALANCE;
                sheet.Cell(currentRow, 12).Style.NumberFormat.Format = "\"Rp\" #,##0";

                sheet.Range(currentRow, 1, currentRow, 12)
                    .Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                    .Border.SetInsideBorder(XLBorderStyleValues.Thin);

                lastSerial = serial;
                currentRow++;
            }

            sheet.ColumnsUsed().AdjustToContents();

            if (mergeStartRow < currentRow - 1)
            {
                foreach (var col in mergeColumns)
                {
                    sheet.Range(mergeStartRow, col, currentRow - 1, col).Merge();
                }
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}