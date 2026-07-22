using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Service.Modules.Core.Service;
using System.Linq;

namespace QCard.ManagementSystem
{
    public class AppAuthorizeAttribute : TypeFilterAttribute
    {
        public AppAuthorizeAttribute(params string[] permissions)
            : base(typeof(AuthorizeActionFilter))
        {
            Arguments = new object[] { permissions };
        }
    }

    public class AuthorizeActionFilter : IAuthorizationFilter
    {
        private readonly string[] _permissions;

        public AuthorizeActionFilter(params string[] permissions)
        {
            _permissions = permissions;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.ActionDescriptor.EndpointMetadata.Any(em => em is AllowAnonymousAttribute))
            {
                return;
            }
            if (_permissions == null || _permissions.Length == 0)
            {
                context.Result = new ForbidResult();
                return;
            }

            var hasPermission = FunctionService.HasPermission(_permissions[0], Feature.VIEW_FEATURE);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
