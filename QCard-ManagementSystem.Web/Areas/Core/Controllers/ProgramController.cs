using ClosedXML.Excel;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Core.Service;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QCard.ManagementSystem.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    [AppAuthorize("ProgramMaintenance")]
    public class ProgramController : WebController
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult AddProgram()
        {
            return View("Component/Add");
        }
        public IActionResult Details(int id)
        {
            var model = Service<ProgramService>().GetProgramsDetail(id);

            return View("Component/View", model);
        }
        public IActionResult Edit(int id)
        {
            var model = Service<ProgramService>().GetProgramsDetailEdit(id);
            var partNoList = Service<ProgramService>().GetProgramMaterial(new DataSourceRequest(), id);

            model.AdditionalData = partNoList.Data;

            return View("Component/Edit", model);
        }
        [HttpGet]
        public IActionResult GetUploadedMaterials(string tempCode)
        {
            return new JsonResult(Service<ProgramService>().GetUploadedMaterials(tempCode));
        }
        [HttpPost]
        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult? result = Service<ProgramService>().GetDataSourceResult(request);
            return Json(result);
        }

        [HttpPost]
        public IActionResult GetProgramMaterial([DataSourceRequest] DataSourceRequest request, int programId)
        {
            DataSourceResult? result = Service<ProgramService>().GetProgramMaterial(request, programId);
            return Json(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetProgramById(long id)
        {
            var result = Service<ProgramService>().GetProgramById(id);
            return Json(new { data = result });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(ProgramRequest program)
        {
            return new JsonResult(Service<ProgramService>().Save(program));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(string id)
        {
            return new JsonResult(Service<ProgramService>().Delete(id));
        }
        public IActionResult DownloadTemplate()
        {
            var stream = new FileStream(@"Areas\Core\Template\Material Detail-Template.xlsx", FileMode.Open);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Material Detail-Template.xlsx");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UploadProcess(IEnumerable<IFormFile> upload)
        {
            var result = Service<ProgramService>().HandleExcelUpload(upload);
            return Json(result);
        }
        public ActionResult ReadUploadedData([DataSourceRequest] DataSourceRequest request, string tempCode)
        {
            DataSourceResult? result = Service<ProgramService>().GetUploadMaterialTemporary(request, tempCode);
            return Json(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitExcelUpload(string tempCode)
        {
            var result = Service<ProgramService>().SubmitExcelUpload(tempCode);
            return Json(result);
        }

        public IActionResult DownloadExcelError(string tempCode)
        {
            var templatePath = Path.Combine(Environment.CurrentDirectory, @"Areas\Core\Template\Material Detail-Template - Error.xlsx");
            var listData = Service<ProgramService>().GetMaterialsUploadValidationByTempCode(tempCode);
            var stream = new MemoryStream();

            using (var workbook = new XLWorkbook(templatePath))
            {
                var worksheet = workbook.Worksheet(1); // index 1-based
                int currentPos = 6;
                int row = currentPos;

                foreach (var data in listData.OrderBy(a => a.NO))
                {
                    worksheet.Cell(row, 1).Value = data.NO;
                    worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 2).Value = data.PART_NO;
                    worksheet.Cell(row, 3).Value = data.UPLOADED_PART_NAME;
                    worksheet.Cell(row, 4).Value = data.QTY;
                    worksheet.Cell(row, 5).Value = data.REGISTERED_PART_NAME;
                    worksheet.Cell(row, 6).Value = data.PART_NAME_MATCH_STATUS;
                    worksheet.Cell(row, 7).Value = data.STATUS;
                    worksheet.Cell(row, 8).Value = data.MESSAGE;

                    for (int col = 1; col <= 8; col++)
                    {
                        worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }

                    row++;
                }

                workbook.SaveAs(stream);
            }

            stream.Position = 0;
            string excelName = $"MaterialsUploadError-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}