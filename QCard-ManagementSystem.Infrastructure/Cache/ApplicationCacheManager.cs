using Agit.Framework.Web;
using System.Configuration;
//using Microsoft.AspNetCore.Http;
using System.Runtime.Caching;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;

namespace QCard.ManagementSystem.Infrastructure.Cache
{
    public static class ApplicationCacheManager
    {
        public static readonly string AppConfigCacheKey = "app_config";
        public static readonly string AppKey = "LOCALGT";
        public static readonly string PermissionCacheKey = "permission_config";
        public static readonly string PermissionMenuCacheKey = "permissionmenu_config";
        private static object _locker = new object();

        public static void GenerateConfigs()
        {
            lock (_locker)
            {
                using (var Db = new DbHelper())
                {
                    var config = Db.ConfigRepository.FindAll();
                    Set(AppConfigCacheKey, config);
                }
            }
        }

        public static void RefreshRolePermissions()
        {
            lock (_locker)
            {
                using (var Db = new DbHelper())
                {
                    var rolePermission = Db.RolePermissionRepository.FindAll();
                    var menu = Db.MenuRepository.Find(new { STATUS = true }).ToList();

                    Set(PermissionCacheKey, rolePermission);
                    Set(PermissionMenuCacheKey, menu);
                }
            }
        }

        public static void RefreshMenu()
        {
            lock (_locker)
            {
                using (var Db = new DbHelper())
                {
                    var menu = Db.MenuRepository.FindAll().ToList();

                    Set(PermissionMenuCacheKey, menu);
                }
            }
        }

        public static T Get<T>(string key)
        {
            var cache = MemoryCache.Default;
            return (T)cache[key];
        }

        public static void Set(string key, object obj)
        {
            var cache = MemoryCache.Default;
            var objCache = cache.Get(key);

            if (objCache != null)
            {
                cache.Remove(key);
            }

            cache[key] = obj;
        }

        public static IEnumerable<RolePermissionView> GetPermissions(IEnumerable<string> roles)
        {
            if (roles == null) return new List<RolePermissionView>();

            var cache = MemoryCache.Default;
            var permissions = cache.Get(PermissionCacheKey) as IEnumerable<RolePermissionView>;

            return permissions.Where(x => roles.Contains(x.ROLE_NAME));
        }

        public static T GetConfig<T>(string /*key*/ SystemType, string /*module*/ SystemID = null)
        {
            var value = string.Empty;

            if (HttpContext.Current != null)
            {
                var cache = MemoryCache.Default;
                var configurations = cache.Get(AppConfigCacheKey) as IEnumerable<Config>;
                Func<Config, bool> expr = null;

                if (!string.IsNullOrEmpty(SystemID))
                    expr = x => x.SYSTEM_TYPE == SystemType && x.SYSTEM_ID == SystemID;
                else
                    expr = x => x.SYSTEM_TYPE == SystemType;

                value = configurations.FirstOrDefault(expr).SYSTEM_VALUE_TEXT;
            }
            else
            {
                value = ConfigurationManager.AppSettings[SystemType];
            }

            return (T)System.Convert.ChangeType(value, typeof(T));
        }

        //public static IEnumerable<GeneralCategory> GetCategories(string category)
        //{
        //    var cache = MemoryCache.Default;

        //    var generalCategories = cache.Get(GeneralCategoryCacheKey) as IEnumerable<GeneralCategory>;

        //    return generalCategories.Where(x => x.Category == category).OrderBy(x => x.Name);
        //}
    }
}
