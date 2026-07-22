using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Service.Modules.Core.Service;
using QCard.ManagementSystem.Service.Modules.QCard.Service;

namespace QCard.ManagementSystem.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    [AppAuthorize("UserMaintenance")]
    public class UserController : WebController
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult? result = Service<UserService>().GetDataSourceResult(request);
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetUserById(string id)
        {
            var result = Service<UserService>().GetUserById(id);
            return Json(new { data = result });
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult CheckQCardByCardNo(string qcardNo)
        {
            var result = Service<QCardListDataService>().CheckQCardByCardNo(qcardNo);
            return new JsonResult(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(UserRequest user)
        {
            return new JsonResult(Service<UserService>().Save(user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(string id)
        {
            return new JsonResult(Service<UserService>().Delete(id));
        }
        public IActionResult DownloadTemplate()
        {
            var stream = new FileStream(@"Areas\Core\Template\User Maintenance-Template.xlsx", FileMode.Open);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "User Maintenance-Template.xlsx");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UploadProcess(IEnumerable<IFormFile> upload)
        {
            var result = Service<UserService>().HandleExcelUpload(upload);
            return Json(result);
        }
        public ActionResult ReadUploadedData([DataSourceRequest] DataSourceRequest request, string tempCode)
        {
            DataSourceResult? result = Service<UserService>().GetUploadUserTemporary(request, tempCode);
            return Json(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitExcelUpload(string tempCode)
        {
            var result = Service<UserService>().SubmitExcelUpload(tempCode);
            return Json(result);
        }
        public IActionResult DownloadExcelError(string tempCode)
        {
            var templatePath = Path.Combine(Environment.CurrentDirectory, @"Areas\Core\Template\UserMaintenance-Template-Error.xlsx");
            var listData = Service<UserService>().GetUserUploadValidationByTempCode(tempCode);
            var stream = new MemoryStream();

            using (var workbook = new XLWorkbook(templatePath))
            {
                var worksheet = workbook.Worksheet(1); // index 1-based
                int currentPos = 11;
                int row = currentPos;

                foreach (var data in listData.OrderBy(a => a.NO))
                {
                    worksheet.Cell(row, 1).Value = data.ACTION;
                    worksheet.Cell(row, 2).Value = data.NO;
                    worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 3).Value = data.UPLOADED_FIRST_NAME;
                    worksheet.Cell(row, 4).Value = data.UPLOADED_LAST_NAME;
                    worksheet.Cell(row, 5).Value = data.UPLOADED_PHONE_NUMBER;
                    worksheet.Cell(row, 6).Value = data.UPLOADED_EMAIL;
                    worksheet.Cell(row, 7).Value = data.PASSWORD;
                    worksheet.Cell(row, 8).Value = data.UPLOADED_ROLE_NAME;
                    worksheet.Cell(row, 9).Value = data.STATUS;
                    worksheet.Cell(row, 10).Value = data.MESSAGE;

                    for (int col = 1; col <= 10; col++)
                    {
                        worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }

                    row++;
                }

                workbook.SaveAs(stream);
            }

            stream.Position = 0;
            string excelName = $"PartNo.UploadError-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}
