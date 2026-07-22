using Kendo.Mvc.UI;
using MathNet.Numerics.RootFinding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Service;
using QCard.ManagementSystem.Service.Modules.Core.Service;

namespace QCard.ManagementSystem.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    [AppAuthorize("ConfigMaintenance")]
    public class ConfigController : WebController
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult();
            result = Service<ConfigService>().GetDataSourceResult(request);
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save()
        {
            var data = Request.Form["data"];
            var configModel = JsonConvert.DeserializeObject<Config>(data);
            return new JsonResult(Service<ConfigService>().Save(configModel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Update()
        {
            var data = Request.Form["data"];
            var configModel = JsonConvert.DeserializeObject<Config>(data);
            return new JsonResult(Service<ConfigService>().Update(configModel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(string systemId, string systemType)
        {
            return new JsonResult(Service<ConfigService>().Delete(systemId, systemType));
        }
    }
}
