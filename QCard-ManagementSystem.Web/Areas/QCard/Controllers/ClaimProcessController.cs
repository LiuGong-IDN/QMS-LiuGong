using DocumentFormat.OpenXml.Office2010.Excel;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Domain.Modules.QCard.Request;
using QCard.ManagementSystem.Service.Modules.Core.Service;
using QCard.ManagementSystem.Service.Modules.MyQCard.Service;
using System;

namespace QCard.ManagementSystem.Web.Areas.QCard.Controllers
{
    [Area("QCard")]
    [Authorize]
    [AppAuthorize("ClaimProcess")]
    public class ClaimProcessController : WebController
    {
        public IActionResult Index(int? status)
        {
            ViewBag.SelectedStatus = status;
            return View();
        }

        [HttpPost]
        public IActionResult Read([DataSourceRequest] DataSourceRequest request, string claimNo, string serialNo, int status)
        {
            DataSourceResult? result = Service<ClaimProcessService>().GetDataSourceResult(request, claimNo, serialNo, status);
            return Json(result);
        }

        [HttpGet]
        public IActionResult ClaimDetailProcess(string claimNo, string claimType)
        {
            string viewDir;
            BasicDataResponse model;
            if (claimType.Equals("program", StringComparison.CurrentCultureIgnoreCase))
            {
                model = Service<ClaimProcessService>().GetClaimProcessDetail(claimNo);
                viewDir = "Component/ClaimProcessProgramDetail";
            }
            else
            {
                model = Service<ClaimProcessService>().GetClaimProcessVoucherDetail(claimNo);
                viewDir = "Component/ClaimProcessVoucherDetail";
            }
            return View(viewDir, model);
        }

        [HttpPost]
        public IActionResult GetClaimStatusHistory([DataSourceRequest] DataSourceRequest request, int claimId)
        {
            DataSourceResult? result = Service<DataSourceService>().GetClaimStatusHistory(request, claimId);
            return Json(result);
        }
        [HttpPost]
        public IActionResult GetProgramDetailHistory(int claimId, int claimDetailId, int programId)
        {
            var result = Service<ClaimHistoryService>().GetProgramDetailHistory(claimId, claimDetailId, programId);
            return Json(result);
        }
        [HttpPost]
        public IActionResult UpdateClaimProcess([FromBody] ClaimProcessRequest request)
        {
            var result = Service<ClaimProcessService>().UpdateClaimProcess(request);
            return Json(result);
        }
        [HttpPost]
        public IActionResult VerifyClaimProcess(ClaimProcessVerifyRequest request, int claimId)
        {
            var result = Service<ClaimProcessService>().VerifyClaimProcess(request, claimId);
            return Json(result);
        }
        [HttpPost]
        public IActionResult UpdatePartClaimQty([FromBody] ClaimProcessUpdatePartQty request)
        {
            var result = Service<ClaimProcessService>().UpdatePartClaimQty(request);
            return Json(result);
        }
        [HttpPost]
        public IActionResult ClaimProcessUpdatePartial(ClaimProcessUpdatePartialRequest request)
        {
            var result = Service<ClaimProcessService>().ClaimProcessUpdatePartial(request);
            return Json(result);
        }
        [HttpGet("download/claim-process")]
        public IActionResult ExportClaimToExcel(string type)
        {
            var bytes = Service<ClaimHistoryService>().ExportClaimToExcel(type);
            var fileName = type switch
            {
                "List" => "ClaimList",
                "All" => "ClaimAll",
                "PendingParts" => "ClaimPendingParts",
                _ => "ClaimUnknown"
            };
            var fileNameFormatted = $"{fileName}_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileNameFormatted
            );
        }
    }
}