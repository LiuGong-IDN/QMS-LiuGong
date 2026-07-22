using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;

namespace QCard.ManagementSystem.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        [NonAction]
        public IActionResult Error(int statusCode)
        {
            switch (statusCode)
            {
                case 400:
                    ViewBag.ErrorMessage = "Bad Request";
                    break;
                case 401:
                    ViewBag.ErrorMessage = "Unauthorized";
                    break;
                case 403:
                    ViewBag.ErrorMessage = "Forbidden";
                    break;
                case 404:
                    ViewBag.ErrorMessage = "Not Found";
                    break;
                case 500:
                    ViewBag.ErrorMessage = "Internal Server Error";
                    break;
                case 502:
                    ViewBag.ErrorMessage = "Bad Gateway";
                    break;
                case 503:
                    ViewBag.ErrorMessage = "Service Unavailable";
                    break;
                default:
                    ViewBag.ErrorMessage = "Unexpected Error Happened";
                    break;
            }
            ViewBag.ErrorHeader = Convert.ToString(statusCode) + " Error";

            return View("CustomError");
        }

        protected IExceptionHandlerFeature Exception { get { return HttpContext.Features.Get<IExceptionHandlerFeature>(); } }

        public IActionResult Index(int statusCode)
        {
            switch (statusCode)
            {
                case 400:
                    ViewBag.ErrorMessage = "Bad Request";
                    break;
                case 401:
                    ViewBag.ErrorMessage = "You are not authorized to access the system. Please contact your administrator.";
                    break;
                case 403:
                    ViewBag.ErrorMessage = "Forbidden";
                    break;
                case 404:
                    ViewBag.ErrorMessage = "Not Found";
                    break;
                case 500:
                    ViewBag.ErrorMessage = "Internal Server Error";
                    break;
                case 502:
                    ViewBag.ErrorMessage = "Bad Gateway";
                    break;
                case 503:
                    ViewBag.ErrorMessage = "Service Unavailable";
                    break;
                default:
                    ViewBag.ErrorMessage = "Unexpected Error Happened";
                    break;
            }
            ViewBag.ErrorHeader = Convert.ToString(statusCode) + " Error";

            return View("CustomError");
        }

        public IActionResult Ajax()
        {
            return BadRequest(new { Exception.Error.Message });
        }
    }
}