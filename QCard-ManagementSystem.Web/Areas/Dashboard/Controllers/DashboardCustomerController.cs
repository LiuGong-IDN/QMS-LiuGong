using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QCard.ManagementSystem.Service.Modules.Dashboard.Service;
using QCard.ManagementSystem.Service.Modules.MyQCard.Service;
using QCard.ManagementSystem.Web;
using System;

namespace Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize]
    public class DashboardCustomerController : WebController
    {
        [HttpGet]
        public IActionResult GetDashboardCustomerSummary()
        {
            var result = Service<DashboardService>().GetDashboardCustomerSummary();
            return new JsonResult(result);
        }
        [HttpPost]
        public IActionResult GetDashboardCustomerLatestClaim()
        {
            var result = Service<DashboardService>().GetDashboardCustomerLatestClaim();
            return new JsonResult(result);
        }
        [HttpGet]
        public IActionResult GetDashboardCustomerRemainingClaim(string serialNo)
        {
            var result = Service<DashboardService>().GetDashboardCustomerRemainingClaim(serialNo);
            return new JsonResult(result);
        }
    }
}