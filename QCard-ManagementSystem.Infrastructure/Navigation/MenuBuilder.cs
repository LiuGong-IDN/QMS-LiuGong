using Agit.Framework.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Infrastructure.Cache;

namespace QCard.ManagementSystem.Infrastructure.Navigation
{
    public class MenuBuilder
    {
        private IEnumerable<RolePermissionView> _acl;
        private IEnumerable<string> _actors;
        private IEnumerable<Menu> _menus;
        private readonly Dictionary<int, Menu> _selectedMenus = new Dictionary<int, Menu>();

        public MenuBuilder(IEnumerable<RolePermissionView> acl, IEnumerable<string> actors, IEnumerable<Menu> menus)
        {
            _acl = acl;
            _actors = actors;
            _menus = menus;
        }

        public HtmlString Render()
        {
            Uri absoluteUri = HttpContext.GetAbsoluteUri();
            DeepSelect(absoluteUri.LocalPath);

            var sb = new StringBuilder();

            var roots = _menus.Where(x => x.PARENT_ID == null).OrderBy(x => x.ORDER_INDEX);

            sb.Append(@"<ul class=""page-sidebar-menu page-sidebar-menu-closed"" data-keep-expanded=""false"" data-auto-scroll=""true"" data-slide-speed=""200"">");
            foreach (var root in roots)
            {
                sb.Append(Render(root));
            }
            sb.Append("</ul>");
            string htmlsidebarmenunew = (sb.ToString())
                                        .Replace("<span class=\"arrow \"></span>", "<div class=\"arrow\" style=\"display:inline-grid; width:10%; float: right;\"><span></span></div>")
                                        .Replace("<span class=\"title\">&nbsp;&nbsp;", "<div class=\"title\" style=\"display:inline-grid; padding-left:3%; width:79%;\"><span>")
                                        .Replace("</span><div class=\"arrow\"", "</span></div><div class=\"arrow\"")
                                        .Replace("</span></a></li><li class=\"nav-item \">", "</span></div></a></li><li class=\"nav-item \">")
                                        .Replace("</span><span class=\"selected\"></span></div>", "</span></div><span class=\"selected\"></span>")
                                        .Replace("</span></a></li><li class=\"nav-item  active open\">", "</span></div></a></li><li class=\"nav-item  active open\">")
                                        .Replace("<span class=\"arrow open\"></span>", "</div><div class=\"arrow open\" style=\"display:inline-grid; width:10%; float: right;\"><span></span></div>")
                                        .Replace("</span></a></li></ul>", "</span></div></a></li></ul>");

            htmlsidebarmenunew = (sb.ToString())
                                        .Replace("<span class=\"new-help-book \"></span>", "<div class=\"new-help-book\"><span></span></div>");

            return new HtmlString(htmlsidebarmenunew);
        }

        private string Render(Menu menu)
        {
            var selected = _selectedMenus.ContainsKey(menu.ID);
            var sb = new StringBuilder();
            var childs = _menus.Where(x => x.PARENT_ID == menu.ID).OrderBy(x => x.ORDER_INDEX);
            var hasChild = childs.Count(x => x.STATUS == true) > 0;

            if (!_actors.Any(x => _acl.Any(y => y.ROLE_NAME == x && y.FUNCTION_ID == menu.FUNCTION_ID)) || menu.STATUS != true) return sb.ToString();

            var urlHelperFactory = HttpContext.Services.GetRequiredService<IUrlHelperFactory>();
            var actionContext = HttpContext.Services.GetRequiredService<IActionContextAccessor>().ActionContext;
            var urlHelper = urlHelperFactory.GetUrlHelper(actionContext);

            sb.Append(string.Format(@"<li class=""nav-item {0}"">", selected ? " active open" : string.Empty));

            sb.Append(string.Format(@"<a href=""{0}"" class=""nav-link {1} {2}"">", !string.IsNullOrEmpty(menu.URL) && !menu.URL.Contains("#") ? urlHelper.Content(menu.URL) : "#", hasChild ? "nav-toggle" : string.Empty, menu.FUNCTION_ID));
            sb.Append(string.Format(@"<i class=""{0}""></i>", menu.ICON_CLASS));
            sb.Append(string.Format(@"<span class=""title title-menu"">&nbsp;&nbsp;{0}</span>", menu.TITLE));

            if (hasChild)
                sb.Append(string.Format(@"<span class=""arrow {0}""></span>", selected ? "open" : string.Empty));

            if (selected)
                sb.Append(@"<span class=""selected""></span>");

            sb.Append(@"</a>");

            if (hasChild)
            {
                sb.Append(@"<ul class=""sub-menu"">");
                foreach (var child in childs)
                {
                    sb.Append(Render(child));
                }
                sb.Append("</ul>");
            }

            sb.Append(@"</li>");

            return sb.ToString();
        }

        private void DeepSelect(string uri)
        {
            var menu = _menus.FirstOrDefault(x => (string.IsNullOrEmpty(x.URL) ? "" : x.URL.Replace("~", "")) == uri.Split(new string[] { "?" }, StringSplitOptions.None)[0]);

            if (menu == null) return;

            while (menu != null)
            {
                _selectedMenus.Add(menu.ID, menu);

                menu = _menus.FirstOrDefault(x => x.ID == menu.PARENT_ID);
            }
        }
    }
}
