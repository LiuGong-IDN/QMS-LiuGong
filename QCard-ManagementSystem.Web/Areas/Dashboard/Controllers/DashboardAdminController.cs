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
    public class DashboardAdminController : WebController
    {
        public IActionResult GetDashboardAdminSummary()
        {
            var result = Service<DashboardService>().GetDashboardAdminSummary();
            return new JsonResult(result);
        }
        [HttpGet]
        public IActionResult GetDashboardAdminQCardStatistics(string currentMonth)
        {
            var result = Service<DashboardService>().GetDashboardAdminQCardStatistics(currentMonth);
            return new JsonResult(result);
        }
        [HttpGet]
        public IActionResult GetDashboardAdminClaimStatistics(string startDateCurrent, string endDateCurrent, string startDatePrev, string endDatePrev)
        {
            var result = Service<DashboardService>().GetDashboardAdminClaimStatistics(startDateCurrent, endDateCurrent, startDatePrev, endDatePrev);
            return new JsonResult(result);
        }
        [HttpGet]
        public IActionResult GetDashboardMostClaimedProgram(string selectedDate)
        {
            var result = Service<DashboardService>().GetDashboardMostClaimedProgram(selectedDate);
            return new JsonResult(result);
        }
        [HttpGet]
        public IActionResult GetDashboardTopCustomer(string selectedDate)
        {
            var result = Service<DashboardService>().GetDashboardTopCustomer(selectedDate);
            return new JsonResult(result);
        }
        [HttpGet]
        public IActionResult GetDashboardTopProductGroup(string selectedDate)
        {
            var result = Service<DashboardService>().GetDashboardTopProductGroup(selectedDate);
            return new JsonResult(result);
        }
        [HttpGet]
        public IActionResult GetDashboardTopProductModel(string selectedDate)
        {
            var result = Service<DashboardService>().GetDashboardTopProductModel(selectedDate);
            return new JsonResult(result);
        }
    }
}