using Microsoft.AspNetCore.Mvc;
using System;
using QCard.ManagementSystem.Service;

namespace QCard.ManagementSystem.Web
{
    public class WebController : Controller
    {
        public IServiceProvider ServiceProvider { get { return HttpContext.RequestServices; } }

        protected T Service<T>() where T : DbService
        {
            return (T)ServiceProvider.GetService(typeof(T));
        }
    }
}
