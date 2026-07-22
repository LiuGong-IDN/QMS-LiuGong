using System.Reflection;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Constants;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Infrastructure.Cache;
using QCard.ManagementSystem.Infrastructure.Session;

namespace QCard.ManagementSystem.Service.Modules.Core.Service
{
    public class FunctionService : DbService
    {
        public FunctionService(IDbHelper db) : base(db)
        {
        }

        public static bool HasPermission(string functionName, Feature feature)
        {
            using (DbHelper db = new DbHelper())
            {
                string methodName = "HasPermission";
                var result = false;
                try
                {
                    var roles = SessionManager.Roles;
                    var permissions = ApplicationCacheManager.GetPermissions(roles);

                    foreach (var perm in permissions)
                    {
                        if (perm.FUNCTION_ID == functionName)
                        {
                            switch (feature)
                            {
                                case Feature.CREATE_FEATURE:
                                    if (perm.CREATE_FEATURE) result = true;
                                    break;
                                case Feature.UPDATE_FEATURE:
                                    if (perm.UPDATE_FEATURE) result = true;
                                    break;
                                case Feature.DELETE_FEATURE:
                                    if (perm.DELETE_FEATURE) result = true;
                                    break;
                                case Feature.UPLOAD_FEATURE:
                                    if (perm.UPLOAD_FEATURE) result = true;
                                    break;
                                case Feature.DOWNLOAD_FEATURE:
                                    if (perm.DOWNLOAD_FEATURE) result = true;
                                    break;
                                case Feature.VIEW_FEATURE:
                                    if (perm.VIEW_FEATURE) result = true;
                                    break;
                                case Feature.SEARCH_FEATURE:
                                    if (perm.SEARCH_FEATURE) result = true;
                                    break;
                            }
                        }
                    }
                    new ActivityLogService(db).CreateSuccessLog(methodName, "FunctionService");
                }
                catch (Exception ex)
                {
                    new ActivityLogService(db).CreateErrorLog(methodName, "FunctionService", ex.Message, ex.StackTrace);
                }

                return result;
            }
        }
    }
}
