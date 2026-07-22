using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Domain.Modules.MyQCard.Request;
using QCard.ManagementSystem.Service.Modules.Core.Service;
using QCard.ManagementSystem.Service.Modules.MyQCard.Service;
using System;

namespace QCard.ManagementSystem.Web.Areas.MyQCard.Controllers
{
    [Area("MyQCard")]
    [Authorize]
    [AppAuthorize("ClaimHistory")]
    public class ClaimHistoryController : WebController
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Read([DataSourceRequest] DataSourceRequest request, string claimNo, string serialNo, int status)
        {
            DataSourceResult? result = Service<ClaimHistoryService>().GetDataSourceResult(request, claimNo, serialNo, status);
            return Json(result);
        }
        [HttpGet]
        public IActionResult ClaimDetail(int id, string categoryType)
        {
            string viewDir;
            BasicDataResponse model;
            if (categoryType.Equals("program", StringComparison.CurrentCultureIgnoreCase))
            {
                model = Service<ClaimHistoryService>().GetClaimProgramDetail(id);
                viewDir = "Component/ClaimProgramDetail";
            }
            else
            {
                model = Service<ClaimHistoryService>().GetClaimVoucherDetail(id);
                viewDir = "Component/ClaimVoucherDetail";
            }
            return View(viewDir, model);
        }
        [HttpPost]
        public IActionResult GetClaimHistoryProgramDetail([DataSourceRequest] DataSourceRequest request, int claimId)
        {
            DataSourceResult? result = Service<ClaimHistoryService>().GetClaimHistoryProgramDetail(request, claimId);
            return Json(result);
        }
        [HttpPost]
        public IActionResult GetClaimHistoryVoucherDetail([DataSourceRequest] DataSourceRequest request, int claimId)
        {
            DataSourceResult? result = Service<ClaimHistoryService>().GetClaimHistoryVoucherDetail(request, claimId);
            return Json(result);
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
            return new JsonResult(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateClaim(UpdateClaimRequest request)
        {
            return new JsonResult(Service<ClaimHistoryService>().UpdateClaim(request));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateClaimVoucher(UpdateClaimVoucherRequest request)
        {
            return new JsonResult(Service<ClaimHistoryService>().UpdateClaimVoucher(request));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmPartialPickup(string claimNo)
        {
            var result = Service<ClaimHistoryService>().ConfirmPartialPickup(claimNo);
            return Json(result);
        }
        [HttpGet("download/claim-history")]
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