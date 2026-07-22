using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Service.Modules.Core.Service;

namespace QCard.ManagementSystem.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    [AppAuthorize("PartNoMaintenance")]
    public class PartNoController : WebController
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Read([DataSourceRequest] DataSourceRequest request, string partNo)
        {
            DataSourceResult? result = Service<PartNoService>().GetDataSourceResult(request, partNo);
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetPartNoById(int id, string partNo)
        {
            var result = Service<PartNoService>().GetPartNoById(id, partNo);
            return Json(new { data = result });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UploadProcess(IEnumerable<IFormFile> upload)
        {
            var result = Service<PartNoService>().HandleExcelUpload(upload);
            return Json(result);
        }
        public ActionResult ReadUploadedData([DataSourceRequest] DataSourceRequest request, string tempCode)
        {
            DataSourceResult? result = Service<PartNoService>().GetUploadPartNoTemporary(request, tempCode);
            return Json(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitExcelUpload(string tempCode)
        {
            var result = Service<PartNoService>().SubmitExcelUpload(tempCode);
            return Json(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(PartNoRequest partno)
        {
            return new JsonResult(Service<PartNoService>().Save(partno));
        }
        public IActionResult DownloadTemplate()
        {
            var stream = new FileStream(@"Areas\Core\Template\Part No. Maintenance - Upload Template.xlsx", FileMode.Open);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Part No. Maintenance-Template.xlsx");
        }
        public IActionResult DownloadExcelError(string tempCode)
        {
            var templatePath = Path.Combine(Environment.CurrentDirectory, @"Areas\Core\Template\Master PartNo-Template-Error.xlsx");
            var listData = Service<PartNoService>().GetPartNoUploadValidationByTempCode(tempCode);
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
                    worksheet.Cell(row, 3).Value = data.UPLOADED_PART_NO;
                    worksheet.Cell(row, 4).Value = data.UPLOADED_PART_NAME;
                    worksheet.Cell(row, 5).Value = data.UPLOADED_RETAIL_PRICE;
                    worksheet.Cell(row, 6).Value = data.STATUS;
                    worksheet.Cell(row, 7).Value = data.MESSAGE;

                    for (int col = 1; col <= 7; col++)
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(string id)
        {
            return new JsonResult(Service<PartNoService>().Delete(id));
        }
    }
}