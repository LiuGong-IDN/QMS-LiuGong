using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Core.Service;
using QCard.ManagementSystem.Domain.Modules.Core.Request;

namespace QCard.ManagementSystem.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    [AppAuthorize("SerialNoMaintenance")]
    public class SerialNoController : WebController
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Add()
        {
            return View("Component/Add");
        }
        public IActionResult Edit(int id)
        {
            var model = Service<SerialNoService>().GetSerialNoDetailsEdit(id);
            var programList = Service<ProgramService>().GetProgramListByMachineId(new DataSourceRequest(), id);

            model.AdditionalData = programList.Data;

            return View("Component/Edit", model);
        }
        public IActionResult Details(int id)
        {
            var model = Service<SerialNoService>().GetSerialNoDetail(id);
            var programList = Service<ProgramService>().GetProgramListByMachineId(new DataSourceRequest(), id);

            model.AdditionalData = programList.Data;
            return View("Component/View", model);
        }
        [HttpPost]
        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult? result = Service<SerialNoService>().GetDataSourceResult(request);
            return Json(result);
        }       
        [HttpPost]
        public IActionResult GetProgramListByMachineId([DataSourceRequest] DataSourceRequest request, int machineId)
        {
            DataSourceResult? result = Service<ProgramService>().GetProgramListByMachineId(request, machineId);
            return Json(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetMachineByID(long id)
        {
            var result = Service<SerialNoService>().GetMachineByID(id);
            return Json(new { data = result });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(MachineRequest machine)
        {
            return new JsonResult(Service<SerialNoService>().Save(machine));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(string id)
        {
            return new JsonResult(Service<SerialNoService>().Delete(id));
        }
    }
}
