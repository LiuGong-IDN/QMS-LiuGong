using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Core.Service;
using QCard.ManagementSystem.Domain.Modules.MyMachines.Request;
using QCard.ManagementSystem.Service.Modules.Core.Service;
using QCard.ManagementSystem.Web;
using System;

namespace QCard.ManagementSystem.Areas.MyMachines.Controllers
{
    [Area("MyMachines")]
    [Authorize]
    [AppAuthorize("MyMachines")]
    public class MyMachinesController : WebController
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Read([DataSourceRequest] DataSourceRequest request, string serialNo, int model, int machineType)
        {
            DataSourceResult? result = Service<MyMachinesService>().GetDataSourceResult(request, serialNo, model, machineType);
            return Json(result);
        }
        [HttpGet]
        public IActionResult MachinesDetailProgram(int id)
        {
            var model = Service<MyMachinesService>().GetMachinesDetail(id);

            return View("Component/MachinesDetailProgram", model);
        }
        [HttpGet]
        public IActionResult MachinesDetailVoucher(int id)
        {
            var model = Service<MyMachinesService>().GetMachinesDetail(id);

            return View("Component/MachinesDetailVoucher", model);
        }

        [HttpPost]
        public IActionResult GetProgramListByMachineId([DataSourceRequest] DataSourceRequest request, int machineId)
        {
            DataSourceResult? result = Service<ProgramService>().GetProgramListByMachineId(request, machineId);
            return Json(result);
        }

        [HttpPost]
        public IActionResult GetProgramDetails(int programId)
        {
            var result = Service<ProgramService>().GetProgramDetailListByProgramId(programId);
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Claim(SubmitClaimRequest request)
        {
            return new JsonResult(Service<MyMachinesService>().Claim(request));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClaimVoucher(SubmitClaimVoucherRequest request)
        {
            return new JsonResult(Service<MyMachinesService>().ClaimVoucher(request));
        }

        [HttpGet("download/machine")]
        public IActionResult ExportMyMachinesToExcel()
        {
            var bytes = Service<MyMachinesService>().ExportMyMachinesToExcel();
            var fileName = "MyMachines";
            var fileNameFormat = fileName + $"_{DateTime.Now:ddMMyyyy}.xlsx";
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileNameFormat
            );
        }
    }
}