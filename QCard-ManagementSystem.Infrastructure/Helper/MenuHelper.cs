using Microsoft.AspNetCore.Html;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Infrastructure.Cache;
using QCard.ManagementSystem.Infrastructure.Navigation;
using QCard.ManagementSystem.Infrastructure.Session;
namespace QCard.ManagementSystem.Infrastructure.Helper
{
    public static class MenuHelpers
    {
        public static HtmlString Sidebar()
        {
            var menus = ApplicationCacheManager.Get<IEnumerable<Menu>>(ApplicationCacheManager.PermissionMenuCacheKey);
            var extendedRoles = new List<string>();
            var roles = SessionManager.Roles.Union(extendedRoles).ToList();
            var rolePermissions = ApplicationCacheManager.Get<IEnumerable<RolePermissionView>>(ApplicationCacheManager.PermissionCacheKey).Where(a => roles.Contains(a.ROLE_NAME));

            var functions = rolePermissions.Where(x => roles.Contains(x.ROLE_NAME) && x.VIEW_FEATURE == true).Select(a => a.FUNCTION_ID).Distinct();

            var sidebarMenu = menus.Where(x => functions.Contains(x.FUNCTION_ID)).ToList();

            var menuBuilder = new MenuBuilder(rolePermissions, roles, sidebarMenu);

            return menuBuilder.Render();
        }

    }
}
