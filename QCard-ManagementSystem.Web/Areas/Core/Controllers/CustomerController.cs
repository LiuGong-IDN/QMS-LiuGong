using ClosedXML.Excel;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Service.Modules.Core.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QCard.ManagementSystem.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    [AppAuthorize("CustomerMaintenance")]
    public class CustomerController(IWebHostEnvironment env) : WebController
    {
        private readonly IWebHostEnvironment _env = env;

        public IActionResult Index()
        {
            return View();
        }

        //GET DATA
        [HttpPost]
        public IActionResult Read([DataSourceRequest] DataSourceRequest request, string companyName, string companyCode)
        {
            DataSourceResult? result = Service<CustomerService>().GetDataSourceResult(request, companyCode, companyName);
            return Json(result);
        }

        //SAVE DATA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(CustomerRequest customer)
        {
            return new JsonResult(Service<CustomerService>().Save(customer));
        }

        //UPDATE DATA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(CustomerRequest customer)
        {
            return new JsonResult(Service<CustomerService>().Update(customer));
        }

        //DELETE DATA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(string id)
        {
            return new JsonResult(Service<CustomerService>().Delete(id));
        }

        [HttpPost]
        public IActionResult GetCustomerByID(long ID)
        {
            try
            {
                var customer = Service<CustomerService>().GetById(ID);
                if (customer == null)
                    return Json(new { success = false, message = "Customer not found" });

                return Json(new { success = true, data = customer });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult DownloadTemplate()
        {
            try
            {
                var filePath = Path.Combine(_env.ContentRootPath, "Areas", "Core", "Template", "Customer Maintenance - Upload Template.xlsx");

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Template file not found");
                }

                var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customer Maintenance - Upload Template.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //UPLOAD PROCESS

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UploadProcess(IEnumerable<IFormFile> upload)
        {
            var result = Service<CustomerService>().HandleExcelUpload(upload);
            return Json(result);
        }

        [HttpPost]
        public IActionResult ReadUploadedData([DataSourceRequest] DataSourceRequest request, string tempCode)
        {
            DataSourceResult? result = Service<CustomerService>().GetUploadCustomerTemporary(request, tempCode);
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitExcelUpload(string tempCode)
        {
            var result = Service<CustomerService>().SubmitExcelUpload(tempCode);
            return Json(result);
        }

        public IActionResult DownloadExcelError(string tempCode)
        {
            var templatePath = Path.Combine(Environment.CurrentDirectory, @"Areas\Core\Template\Customer Maintenance-Template-Error.xlsx");
            var listData = Service<CustomerService>().GetCustomerUploadValidationByTempCode(tempCode);
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
                    worksheet.Cell(row, 3).Value = data.UPLOADED_COMPANY_CODE;
                    worksheet.Cell(row, 4).Value = data.UPLOADED_COMPANY_NAME;
                    worksheet.Cell(row, 5).Value = data.UPLOADED_ADDRESS;
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
            string excelName = $"Customer.UploadError-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}

