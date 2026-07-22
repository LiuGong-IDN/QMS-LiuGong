using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Service.Modules.Core.Service;
using System.Linq;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Web.Areas.Core.Controllers
{
    [Area("Core")]
    public class DataSourceController : WebController
    {
        private readonly IWebHostEnvironment _env;

        public DataSourceController(IWebHostEnvironment env)
        {
            _env = env;
        }
        public IActionResult GetCustomerList(string customer, string filter)
        {
            List<DropDownModel>? result = Service<DataSourceService>().GetCustomerList(customer, filter);
            return Json(result);
        }
        public IActionResult GetClaimStatusList(string filter)
        {
            List<DropDownModel>? result = Service<DataSourceService>().GetClaimStatusList(filter);
            return Json(result);
        }
        public IActionResult GetClaimProcessStatusList(string filter, int status)
        {
            List<DropDownModel>? result = Service<DataSourceService>().GetClaimProcessStatusList(filter, status);
            return Json(result);
        }
        public IActionResult GetRoleList(string filter)
        {
            List<DropDownModel>? result = Service<DataSourceService>().GetRoleList(filter);
            return Json(result);
        }
        public IActionResult GetAssetType(string filter)
        {
            List<DropDownModel>? result = Service<DataSourceService>().GetAssetType(filter);
            return Json(result);
        }
        public IActionResult GetModelList(string filter)
        {
            List<DropDownModel>? result = Service<DataSourceService>().GetModelList(filter);
            return Json(result);
        }
        public IActionResult GetPartNoList(string filter, string partId)
        {
            if (!string.IsNullOrEmpty(partId))
            {
                List<DropDownModel>? partNo = Service<DataSourceService>().GetPartNoById(partId);
                return Json(partNo);
            }
            List<DropDownModel>? result = Service<DataSourceService>().GetPartNoList(filter);
            return Json(result);
        }
        public IActionResult GetModelbyAssetTypeList(long assetTypeId, string filter)
        {
            List<DropDownModel>? result = Service<DataSourceService>().GetModelbyAssetTypeList(assetTypeId, filter);
            return Json(result);
        }
        public IActionResult GetPriorityList(string filter)
        {
            List<DropDownModel>? result = Service<DataSourceService>().GetPriorityList(filter);
            return Json(result);
        }
        public IActionResult GetServicePartyList(string filter)
        {
            List<DropDownModel>? result = Service<DataSourceService>().GetServicePartyList(filter);
            return Json(result);
        }
        public IActionResult GetProgramList(string filter)
        {
            List<DropDownModel>? result = Service<DataSourceService>().GetProgramList(filter);
            return Json(result);
        }
        public IActionResult GetProgramListByModelId(string filter, long modelId)
        {
            List<DropDownModel>? result = Service<DataSourceService>().GetProgramListByModelId(filter, modelId);
            return Json(result);
        }
        public IActionResult GetPDC(string filter)
        {
            List<DropDownModel>? result = Service<DataSourceService>().GetPDC(filter);
            return Json(result);
        }
        public IActionResult GetProgramCategory(string filter)
        {
            List<DropDownModel>? result = Service<DataSourceService>().GetProgramCategory(filter);
            return Json(result);
        }
        public IActionResult GetShippingAgent(string filter)
        {
            List<DropDownModel>? result = Service<DataSourceService>().GetShippingAgent(filter);
            return Json(result);
        }
        
        public IActionResult GetSerialNoQCard(string filter, int customerId, string serialno)
        {
            try
            {
                var result = Service<DataSourceService>().GetSerialNoQCard(filter, customerId, serialno);
                return Json(result);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400; // kirim sebagai Bad Request
                return Json(new { ExistsFlag = 1, Message = ex.Message });
            }
        }

        public async Task<ActionResult> UploadExcel(IEnumerable<IFormFile> upload)
        {
            return Content(string.Empty);
        }
        public async Task<ActionResult> GetFileImage(string fileCategory, string fileCategoryId)
        {
            var result = Service<DataSourceService>().GetFileImage(fileCategory, fileCategoryId);

            if (string.IsNullOrEmpty(result))
                result = "/img/liugongQCard.png"; // pastikan path benar untuk web

            // Gunakan property yang sama dengan JS: imageUrl
            return Json(new { imageUrl = result });
        }
        [HttpPost]
        public async Task<IActionResult> UploadEvidence(IFormFile EvidenceFile)
        {
            if (EvidenceFile == null || EvidenceFile.Length == 0)
                return BadRequest(new { success = false, message = "No file uploaded." });

            try
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "img", "ApprovalEvidence");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Pakai nama file asli (tanpa ubah)
                string fileName = Path.GetFileName(EvidenceFile.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await EvidenceFile.CopyToAsync(stream);
                }

                return Ok(new
                {
                    success = true,
                    message = "File uploaded successfully!",
                    fileName = fileName // ⬅️ kirim hanya nama file
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Upload failed.", error = ex.Message });
            }
        }

        public async Task<ActionResult> GetActionName(string systemType)
        {
            try
            {
                // Panggil service (misal DataSourceService)
                var result = Service<DataSourceService>().GetActionName(systemType);

                if (result == null)
                {
                    // Kalau tidak ketemu data di DB, kembalikan pesan default
                    return Json(new { systemValueText = "", message = "Action name not found" });
                }

                // Return JSON dengan property yang sama seperti di JavaScript
                return Json(new
                {
                    systemType = result.SYSTEM_TYPE,
                    systemValueText = result.SYSTEM_VALUE_TEXT,
                    systemDesc = result.SYSTEM_DESC
                });
            }
            catch (Exception ex)
            {
                // Tangani error agar tidak crash
                return Json(new { systemValueText = "", message = $"Error: {ex.Message}" });
            }
        }
    }
}
