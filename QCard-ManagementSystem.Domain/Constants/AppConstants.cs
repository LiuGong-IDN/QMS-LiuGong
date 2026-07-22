using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;

namespace QCard.ManagementSystem.Domain.Constants
{
    public static class AppConstants
    {
        public const string SessionKey = "AppUserSession";
        public const string SessionTime = "AppUserSessionTime";
        public const string TokenSessionKey = "AppTokenSession";
        public const string NoRecordsText = "No records yet";
        public const string ApplicationName = "QCard_SystemManagement";

        public class LogProcess
        {
            public static readonly string AppName = "QCard_SystemManagement";

            public static readonly string LogRegistration = "Registration";
            public static readonly string LogCategoryDataChanges = "Data Changes";
            public static readonly string LogCategoryWorkerSendEmail = "Worker Service Send Email";
            public static readonly string LogCategoryWorkerBatchUpdate = "Worker Service Batch Update";

            public static readonly string LogAuthentication = "Authentication";
        }
        
        public class ActivityProcess
        {
            public static readonly string REGISTRATION = "Registration";
            public static readonly string CREATE = "Create";
            public static readonly string UPDATE = "Update";
            public static readonly string DELETE = "Delete";
            public static readonly string UPLOAD = "Upload";
            public static readonly string SUBMIT_UPLOAD = "Submit Upload";
            public static readonly string DOWNLOAD = "Download";
            public static readonly string SEARCH = "Search";
            public static readonly string BATCH = "Batch";
            public static readonly string SCHEDULER = "Scheduler";
            public static readonly string LOGIN = "Login";
            public static readonly string LOGOUT = "Logout";
            public static readonly string SEND_EMAIL_SCHEDULER = "Send Email Scheduler";
            public static readonly string BATCH_UPDATE = "Batch Update";
        }

        public class ValidatingDate
        {
            public static readonly string ExpiredEtdDate = "2999-12-31";
        }
    }
}
