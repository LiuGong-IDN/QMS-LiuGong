using Dapper;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Diagnostics;
using QCard.ManagementSystem.Domain.Constants;
using QCard.ManagementSystem.Domain.Enums;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface ILogActivityRepository : IDapperRepository<LogActivity>
    {
        void Save(LogActivity logActivity);
    }

    public partial class LogActivityRepository : DapperRepository<LogActivity>, ILogActivityRepository
    {
        public LogActivityRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {
        }

        public void Save(LogActivity logActivity)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@AppName", logActivity.APPLICATION_NAME);
            parameters.Add("@LogCategory", logActivity.LOG_CATEGORY);
            parameters.Add("@Activity", logActivity.ACTIVITY);
            parameters.Add("@AppModule", logActivity.APPLICATION_MODULE);
            parameters.Add("@Username", logActivity.USERNAME);
            parameters.Add("@CreatedDateTime", logActivity.CREATED_DATETIME);
            parameters.Add("@Hostname", logActivity.IP_HOSTNAME);
            parameters.Add("@Status", logActivity.STATUS);
            parameters.Add("@AdditionalInformation", logActivity.ADDITIONAL_INFORMATION);
            parameters.Add("@StackTrace", logActivity.STACKTRACE);

            Connection.Execute("[dbo].[USP_CREATE_ACTIVITY_LOG]", parameters, null, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}

