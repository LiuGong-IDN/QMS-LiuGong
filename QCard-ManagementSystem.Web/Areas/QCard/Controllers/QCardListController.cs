using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QCard.ManagementSystem.Domain.Modules.QCard.Request;
using QCard.ManagementSystem.Service.Modules.QCard.Service;
using QCard.ManagementSystem.Web.Helpers;
using Rotativa.AspNetCore;

namespace QCard.ManagementSystem.Web.Areas.QCard.Controllers
{
    [Area("QCard")]
    [Authorize]
    [AppAuthorize("QCardList")]
    public class QCardListController : WebController
    {
        private readonly IWebHostEnvironment _env;
        public QCardListController(IWebHostEnvironment env)
        {
            _env = env;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Print(string json)
        {
            var model = Service<QCardListDataService>().GetSelectedQCardPrint(json);
            var qcardData = model.Data;

            var contentRoot = _env.ContentRootPath;

            string frontImagePath = Path.Combine(_env.WebRootPath, "img", "QCardImage", "QCard_Front_2.svg");
            string backImagePath = Path.Combine(_env.WebRootPath, "img", "QCardImage", "QCard_Back.svg");

            string frontImage = SvgHelper.SvgToBase64Png(frontImagePath);
            string backImage = SvgHelper.SvgToBase64Png(backImagePath);

            ViewData["FrontImage"] = frontImage;
            ViewData["BackImage"] = backImage;

            // Ambil orientation dari service (yang baca DB lewat repository)
            string orientationText = Service<QCardListDataService>().GetPdfOrientation();
            var orientation = ConvertOrientation(orientationText);

            return new ViewAsPdf("Component/_PrintQCard", model)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = orientation,
                FileName = $"Q-Card Print-{DateTime.Now:yyyyMMddHHmmssfff}.pdf"
            };
        }
        public Rotativa.AspNetCore.Options.Orientation ConvertOrientation(string orientationText)
        {
            return orientationText?.ToLower() switch
            {
                "portrait" => Rotativa.AspNetCore.Options.Orientation.Portrait,
                "landscape" => Rotativa.AspNetCore.Options.Orientation.Landscape,
                _ => Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }

        [HttpGet]
        public JsonResult GetAllQCardIds()
        {
            try
            {
                // Ambil semua ID dari database
                var allIds = Service<QCardListDataService>().GetAllQCardIds();

                return Json(new { success = true, data = allIds });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public IActionResult Topup(string id)
        {
            var result = Service<QCardListDataService>().GetQCardById(id);
            if (result == null)
            {
                return NotFound();
            }

            return View("Component/_TopupQCard", result);
        }
        public IActionResult Details(int id)
        {
            var model = Service<QCardListDataService>().GetQCardDetail(id);

            return View("Component/_DetailQCard", model);
        }
        [HttpPost]
        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult? result = Service<QCardListDataService>().GetDataSourceResult(request);
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetQCardById(string id)
        {
            var result = Service<QCardListDataService>().GetQCardById(id);
            return Json(new { data = result });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(QCardRequest qcard)
        {
            return new JsonResult(Service<QCardListDataService>().Save(qcard));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveTopup(TopupRequest topup)
        {
            return new JsonResult(Service<QCardListDataService>().SaveTopup(topup));
        }
        public IActionResult DownloadTemplate()
        {
            var stream = new FileStream(@"Areas\Core\Template\TopupQCard-Template.xlsx", FileMode.Open);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TopupQCard-Template.xlsx");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UploadProcess(IEnumerable<IFormFile> upload)
        {
            var result = Service<QCardListDataService>().HandleExcelUpload(upload);
            return Json(result);
        }
        public ActionResult ReadUploadedData([DataSourceRequest] DataSourceRequest request, string tempCode)
        {
            DataSourceResult? result = Service<QCardListDataService>().GetUploadSerialNoTemporary(request, tempCode);
            return Json(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitExcelUpload(string tempCode)
        {
            var result = Service<QCardListDataService>().SubmitExcelUpload(tempCode);
            return Json(result);
        }
        [HttpPost]
        public IActionResult GetSerialNoQCard([DataSourceRequest] DataSourceRequest request, int cardId)
        {
            DataSourceResult? result = Service<QCardListDataService>().GetSerialNoQCard(request, cardId);
            return Json(result);
        }
        public IActionResult DownloadExcelError(string tempCode)
        {
            var templatePath = Path.Combine(Environment.CurrentDirectory, @"Areas\Core\Template\TopupQCard-Template-Error.xlsx");
            var listData = Service<QCardListDataService>().GetSerialNoUploadValidationByTempCode(tempCode);
            var stream = new MemoryStream();

            using (var workbook = new XLWorkbook(templatePath))
            {
                var worksheet = workbook.Worksheet(1);
                int currentPos = 6;
                int row = currentPos;

                foreach (var data in listData.OrderBy(a => a.NO))
                {
                    worksheet.Cell(row, 1).Value = data.NO;
                    worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 2).Value = data.SERIAL_NO;
                    worksheet.Cell(row, 3).Value = data.UPLOADED_PRODUCT_GROUP;
                    worksheet.Cell(row, 4).Value = data.UPLOADED_PRODUCT_MODEL;
                    worksheet.Cell(row, 5).Value = data.PURCHASE_DATE;
                    worksheet.Cell(row, 6).Value = data.REGISTERED_PRODUCT_GROUP;
                    worksheet.Cell(row, 7).Value = data.REGISTERED_PRODUCT_MODEL;
                    worksheet.Cell(row, 8).Value = data.SERIAL_NO_MATCH_STATUS;
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
            string excelName = $"SerialNoUploadError-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        [HttpGet]
        public IActionResult GetUploadedSerialNo(string tempCode)
        {
            return new JsonResult(Service<QCardListDataService>().GetUploadedSerialNo(tempCode));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(string id)
        {
            return new JsonResult(Service<QCardListDataService>().Delete(id));
        }
    }
}