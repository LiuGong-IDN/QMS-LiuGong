using ExcelDataReader;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Constants;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Infrastructure.General;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Models;
using QCard.ManagementSystem.Service;
using QCard.ManagementSystem.Service.Modules.Core.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    [AppAuthorize("RoleMaintenance")]
    public class RoleController : WebController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();
            result = Service<RoleService>().GetDataSourceResult(request);
            return Json(result);
        }

        public IActionResult ReadRolePermission([DataSourceRequest] DataSourceRequest request, string roleName, string functionName)
        {
            var result = new DataSourceResult();
            result = Service<RoleService>().GetRolePermissionList(roleName, functionName);
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePermission()
        {
            var data = Request.Form["data"];
            var RolePermissionModel = JsonConvert.DeserializeObject<List<RolePermissionView>>(data);
            return new JsonResult(Service<RoleService>().Update(RolePermissionModel));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(Role role)
        {
            return new JsonResult(Service<RoleService>().Save(role));
        }
    }
}
