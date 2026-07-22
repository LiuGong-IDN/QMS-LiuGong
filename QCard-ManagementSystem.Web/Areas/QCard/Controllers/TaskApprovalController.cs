using System;
using System.Collections.Generic;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Modules.Core.Service;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.QCard.Model;
using QCard.ManagementSystem.Domain.Modules.QCard.Request;
using QCard.ManagementSystem.Service.Modules.QCard.Service;

namespace QCard.ManagementSystem.Web.Areas.QCard.Controllers
{
    [Area("QCard")]
    [Authorize]
    [AppAuthorize("TaskApprovalQCard")]
    public class TaskApprovalController : WebController
    {
        private readonly IWebHostEnvironment _env;
        public TaskApprovalController(IWebHostEnvironment env)
        {
            _env = env;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Read([DataSourceRequest] DataSourceRequest request, string cardNo, int customer)
        {
            DataSourceResult? result = Service<TaskApprovalService>().GetDataSourceResult(request, cardNo, customer);
            return Json(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]  
        public IActionResult GetTaskApprovalById(int id)
        {
            var result = Service<TaskApprovalService>().GetTaskApprovalById(id);
            return Json(new { data = result });
        }

        [HttpGet]
        public JsonResult GetAllTaskApprovalIds()
        {
            {
                // Ambil semua ID dari database
                var result = Service<TaskApprovalService>().GetAllTaskApprovalIds();

                return Json(result);
            }
        }
        public IActionResult Details(int id)
        {
            var model = Service<TaskApprovalService>().GetTopupDetail(id);

            if (model?.Data == null)
            {
                return NotFound();
            }

            // 🔑 CAST DI SINI
            var header = model.Data as ApprovalDetail;

            if (header == null)
            {
                throw new Exception("Model.Data is not ApprovalDetail");
            }

            var machineId = header.MACHINE_ID;

            if (machineId > 0)
            {
                var programList = Service<ProgramService>()
                    .GetProgramListByMachineId(new DataSourceRequest(), machineId);

                model.AdditionalData = programList?.Data;
            }
            else
            {
                model.AdditionalData = new List<MachineProgram>();
            }

            return View("Component/_DetailApproval", model);
        }

        [HttpPost]
        public IActionResult GetProgramListByMachineId([DataSourceRequest] DataSourceRequest request, int machineId)
        {
            var result = Service<ProgramService>().GetProgramListByMachineId(request, machineId);
            return Json(result);
        }

        public IActionResult GetApprovalHistoryDetail(long approvalHistoryId)
        {
            var result = Service<TaskApprovalService>().GetApprovalHistoryDetail(approvalHistoryId);

            return Json(result);
        }
        [HttpPost]
        public IActionResult GetApprovalHistory([DataSourceRequest] DataSourceRequest request, int documentId)
        {
            DataSourceResult? result = Service<TaskApprovalService>().GetApprovalHistory(request, documentId);
            return Json(result);
        }
        [HttpGet]
        public IActionResult ExportTaskApprovalToExcel()
        {
            var bytes = Service<TaskApprovalService>().ExportTaskApprovalToExcel();
            string fileName = $"TaskApprovalAll_{DateTime.Now:ddMMyyyy}.xlsx";

            return File(bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(ApprovalRequest approve)
        {
            // EvidenceFile akan otomatis terisi jika ada
            return new JsonResult(Service<TaskApprovalService>().Save(approve));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(ApprovalRequest Reject)
        {
            // EvidenceFile akan otomatis terisi jika ada
            return new JsonResult(Service<TaskApprovalService>().Reject(Reject));
        }
    }
}
