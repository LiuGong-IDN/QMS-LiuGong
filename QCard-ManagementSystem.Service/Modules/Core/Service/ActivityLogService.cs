using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using QCard.ManagementSystem.Domain;
using System.Transactions;
using Dapper;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using QCard.ManagementSystem.Domain.Constants;
using Microsoft.Extensions.Hosting;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;

namespace QCard.ManagementSystem.Service.Modules.Core.Service
{
    public class ActivityLogService : DbService
    {
        public ActivityLogService(IDbHelper db) : base(db)
        {
        }

        public void CreateErrorLog(string activity, string moduleName, string message, string stackTrace = null)
        {
            CreateLog("Failed", activity, moduleName, message,stackTrace);
        }

        public void CreateSuccessLog(string activity, string moduleName, string message = null)
        {
            CreateLog("Success", activity, moduleName, message);
        }

        public void CreateLog(string status, string activity, string moduleName, string message, string stackTrace = null)
        {
            // Host / IP fallback
            string host = HttpContextHelper.Current != null
                ? $"{HttpContextHelper.Current.Request.Scheme}://{HttpContextHelper.Current.Request.Host}{HttpContextHelper.Current.Request.PathBase}"
                : "System";

            // Log category
            string logCategory;

            if (activity == AppConstants.ActivityProcess.LOGIN || activity == AppConstants.ActivityProcess.LOGOUT)
            {
                logCategory = AppConstants.LogProcess.LogAuthentication;
            }
            else if (activity == AppConstants.ActivityProcess.BATCH_UPDATE)
            {
                logCategory = AppConstants.LogProcess.LogCategoryWorkerBatchUpdate;
            }
            else if (activity == AppConstants.ActivityProcess.SEND_EMAIL_SCHEDULER)
            {
                logCategory = AppConstants.LogProcess.LogCategoryWorkerSendEmail;
            }
            else if (activity == AppConstants.ActivityProcess.REGISTRATION)
            {
                logCategory = AppConstants.LogProcess.LogRegistration;
            }
            else
            {
                logCategory = AppConstants.LogProcess.LogCategoryDataChanges;
            }


            // Current user
            string currentUser = SessionManager.Current ?? "System";

            // Build log
            var logActivity = new LogActivity
            {
                APPLICATION_NAME = AppConstants.LogProcess.AppName,
                LOG_CATEGORY = logCategory,
                ACTIVITY = activity,
                APPLICATION_MODULE = moduleName,
                USERNAME = currentUser,
                CREATED_DATETIME = DateTime.Now,
                IP_HOSTNAME = host,
                STATUS = status,
                ADDITIONAL_INFORMATION = message,
                STACKTRACE = stackTrace
            };

            Db.LogActivityRepository.Save(logActivity);
        }

    }
}
