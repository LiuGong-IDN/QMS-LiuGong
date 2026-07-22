using Microsoft.AspNetCore.Mvc;

namespace QCard.ManagementSystem.Infrastructure.Extension
{
    public static class ControllerContextExtension
    {
        public static ContextInfo GetContextInfo(this ControllerContext controllerContext)
        {
            var routeData = controllerContext.RouteData;

            string area = routeData.DataTokens["area"] != null ? routeData.DataTokens["area"].ToString() : string.Empty,
                   controller = routeData.Values["controller"].ToString(),
                   action = routeData.Values["action"].ToString();

            var contextInfo = new ContextInfo(area, controller, action);

            return contextInfo;
        }
    }
}
