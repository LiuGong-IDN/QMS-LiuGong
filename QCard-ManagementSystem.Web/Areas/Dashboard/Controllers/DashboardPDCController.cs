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
    public class DashboardPDCController : WebController
    {
        public IActionResult GetDashboardPDCSummary()
        {
            var result = Service<DashboardService>().GetDashboardPDCSummary();
            return new JsonResult(result);
        }
        [HttpGet]
        public IActionResult GetDashboardPDCClaimStatistics(string startDateCurrent, string endDateCurrent, string startDatePrev, string endDatePrev)
        {
            var result = Service<DashboardService>().GetDashboardPDCClaimStatistics(startDateCurrent, endDateCurrent, startDatePrev, endDatePrev);
            return new JsonResult(result);
        }
        [HttpGet]
        public IActionResult GetDashboardPDCMostClaimedProgram(string selectedDate)
        {
            var result = Service<DashboardService>().GetDashboardPDCMostClaimedProgram(selectedDate);
            return new JsonResult(result);
        }
        [HttpGet]
        public IActionResult GetDashboardPDCTopCustomer(string selectedDate)
        {
            var result = Service<DashboardService>().GetDashboardPDCTopCustomer(selectedDate);
            return new JsonResult(result);
        }
    }
}