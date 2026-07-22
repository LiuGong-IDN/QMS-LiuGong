using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QCard.ManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using QCard.ManagementSystem.Web;
using Kendo.Mvc.UI;
using QCard.ManagementSystem.Service;
using Microsoft.AspNetCore.Http;
using QCard.ManagementSystem.Infrastructure;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Domain.Modules;
using QCard.ManagementSystem.Domain;
using System.IO;
using OfficeOpenXml;
using System.Globalization;
using OfficeOpenXml.Style;
using QCard.ManagementSystem.Infrastructure.General;

namespace QCard.ManagementSystem.Controllers
{
    [Authorize]
    public class HomeController : WebController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var username = SessionManager.Current;
            ViewBag.LoginUserRole = SessionManager.RoleStr;
            //ViewBag.NoDataFound = Service<MessageService>().GetMessagebyID(EnumHelper.GetDescription(Domain.Enums.Message.NoDataFound)).MESSAGE_TEXT;

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
