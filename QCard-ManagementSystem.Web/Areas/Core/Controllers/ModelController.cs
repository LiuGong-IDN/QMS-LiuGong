using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Service.Modules.Core.Service;

namespace QCard.ManagementSystem.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    [AppAuthorize("ModelMaintenance")]
    public class ModelController : WebController
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult? result = Service<ModelService>().GetDataSourceResult(request);
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetModelById(string id, string modelCode, int assetType)
        {
            var result = Service<ModelService>().GetModelById(id, modelCode, assetType);
            return Json(new { data = result });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(ModelRequest model)
        {
            return new JsonResult(Service<ModelService>().Save(model));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(string id)
        {
            return new JsonResult(Service<ModelService>().Delete(id));
        }
    }
}
