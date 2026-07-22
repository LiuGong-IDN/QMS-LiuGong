using Kendo.Mvc.UI;
using Kendo.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Infrastructure.Cache;
using QCard.ManagementSystem.Service;
using QCard.ManagementSystem.Service.Modules.Core.Service;

namespace QCard.ManagementSystem.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    [AppAuthorize("MenuMaintenance")]
    public class MenuController : WebController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetListMenu()
        {
            List<Domain.Modules.Core.Model.Menu> menus = new List<Domain.Modules.Core.Model.Menu>();
            var dataSourceResult = Service<MenuService>().GetMenuDataSource();
            menus = JsonConvert.DeserializeObject<List<Domain.Modules.Core.Model.Menu>>(JsonConvert.SerializeObject(dataSourceResult.Data));

            var menuData = menus.Select(p => new
            {
                MenuId = p.ID,
                ParentId = p.PARENT_ID,
                MenuTitle = p.TITLE,
                Url = p.URL,
                Description = p.DESCRIPTION,
                IconClass = p.ICON_CLASS,
                OrderIndex = p.ORDER_INDEX,
                FunctionID = p.FUNCTION_ID,
                Status = p.STATUS,
            });

            return new JsonResult(menuData);
        }

        [HttpPost]
        public JsonResult Save()
        {
            var data = Request.Form["data"];
            var menuModel = JsonConvert.DeserializeObject<Domain.Modules.Core.Model.Menu>(data);
            return new JsonResult(Service<MenuService>().Save(menuModel));
        }

        [HttpPost]
        public JsonResult Update()
        {
            var data = Request.Form["data"];
            var menuModel = JsonConvert.DeserializeObject<Domain.Modules.Core.Model.Menu>(data);
            return new JsonResult(Service<MenuService>().Update(menuModel));
        }

        [HttpPost]
        public JsonResult Delete()
        {
            var data = Request.Form["data"];
            var menuModel = JsonConvert.DeserializeObject<Domain.Modules.Core.Model.Menu>(data);
            return new JsonResult(Service<MenuService>().Delete(menuModel));
        }

        [HttpPost]
        public JsonResult MoveMenu()
        {
            var data = Request.Form["data"];
            var menuModel = JsonConvert.DeserializeObject<Domain.Modules.Core.Model.Menu>(data);
            return new JsonResult(Service<MenuService>().MoveMenu(menuModel));
        }
    }
}
