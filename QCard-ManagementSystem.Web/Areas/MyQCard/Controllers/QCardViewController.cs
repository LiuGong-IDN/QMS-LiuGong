using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.MyQCard.ViewModel;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Service.Modules.MyQCard.Service;

namespace QCard.ManagementSystem.Web.Areas.MyQCard.Controllers
{
    [Area("MyQCard")]
    [Authorize]
    [AppAuthorize("QCardView")]
    public class QCardViewController : WebController
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetQCard()
        {
            var result = Service<QCardViewService>().GetViewQCardByEmail();
            return new JsonResult(result);
        }
    }
}
