using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace QCard.ManagementSystem.Hangfire.Filters
{
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            //Can use this for NetCore
            //return context.GetHttpContext().User.Identity.IsAuthenticated;
            return true;
        }
    }
}
