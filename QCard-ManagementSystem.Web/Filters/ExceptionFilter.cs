using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Service;

namespace QCard.ManagementSystem.Web.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IModelMetadataProvider _modelMetadataProvider;

            public ExceptionFilter(IModelMetadataProvider modelMetadataProvider)
        {
            _modelMetadataProvider = modelMetadataProvider;
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception != null)
            {
                if (context.HttpContext.Request.IsAjaxRequest())
                {
                    var exception = context.Exception;

                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.ExceptionHandled = true;

                    var title = exception.Data.Contains("Title") ? exception.Data["Title"].ToString() : string.Empty;
                    var message = exception.InnerException != null ? exception.InnerException.Message : exception.Message;

                    context.Result = new JsonResult(new ExceptionInfo(title, message));
                }
                else
                {
                    base.OnException(context);
                }
            }
        }
    }

    public class ExceptionInfo
    {
        public bool Exception { get; set; } = true;
        public string Title { get; set; }
        public string Message { get; set; }

        public ExceptionInfo(string title, string message)
        {
            Title = title;
            Message = message;
        }
    }
}
