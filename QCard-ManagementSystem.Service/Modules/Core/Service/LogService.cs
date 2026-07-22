//using TAM.WRH.Infrastructure.Session;

using Dapper;
using Hangfire.Server;
using Kendo.Mvc.UI;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using System.Data;
using System.Transactions;
using TAM.WRH.Domain;
using TAM.WRH.Domain.Constants;
using TAM.WRH.Domain.Enums;
using TAM.WRH.Domain.Modules.Core.Model;
using TAM.WRH.Infrastructure.General;

namespace TAM.WRH.Service
{
    public class LogService : DbService
    {
        private readonly ILogger<Worker> _logger;
        public LogService(IDbHelper db) : base(db)
        {
        }

        public DataSourceResult GetDataSourceResult(DataSourceRequest request, string WhereClause, string OrderBy)
        {
            var query = "usp_Log_WithPaging";

            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GetCount");
            parameters.Add("@WhereClause", WhereClause);
            parameters.Add("@PageNumber", request.Page);
            parameters.Add("@PageSize", request.PageSize);
            parameters.Add("@OrderBy", OrderBy);
            var totalCount = Convert.ToInt32(Db.Connection.ExecuteScalar(query, parameters, commandType: System.Data.CommandType.StoredProcedure));

            parameters.Add("@Type", "GetPaged");
            parameters.Add("@WhereClause", WhereClause);
            parameters.Add("@PageNumber", request.Page);
            parameters.Add("@PageSize", request.PageSize);
            parameters.Add("@OrderBy", OrderBy);
            var _result = Db.Connection.Query<ApplicationLog>(query, parameters, commandType: System.Data.CommandType.StoredProcedure).ToList();

            return new DataSourceResult { Data = _result, Total = totalCount };
        }

        public bool LogProcess(string username, string moduleId, string host, string activity, string status, string errorMsg)
        {
            bool result = false;
            try
            {
                if (string.IsNullOrEmpty(host))
                {
                    host = HttpContextHelper.AppBaseUrl;
                }
            }
            catch (Exception)
            {

                host = "System";
            }

            if (!string.IsNullOrEmpty(activity))
            {
                using (TransactionScope transcope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        var isValid = false;
                        var logCategory = string.Empty;

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
                        else
                        {
                            logCategory = AppConstants.LogProcess.LogCategoryDataChanges;
                        }

                        var parameters = new DynamicParameters();
                        parameters.Add("@AppName", AppConstants.LogProcess.AppName);
                        parameters.Add("@LogCategory", logCategory);
                        parameters.Add("@Activity", activity);
                        parameters.Add("@AppModule", moduleId);
                        parameters.Add("@Username", username);
                        parameters.Add("@CreatedDateTime", DateTime.Now);
                        parameters.Add("@Hostname", host);
                        parameters.Add("@Status", status);
                        parameters.Add("@AdditionalInformation", errorMsg);
                        parameters.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);
                        parameters.Add("@ErrorMsg", errorMsg, System.Data.DbType.String, System.Data.ParameterDirection.Output, 4000);

                        Db.Connection.Execute("[dbo].[USP_CREATE_LOG]", parameters, Db.Transaction, commandType: System.Data.CommandType.StoredProcedure);

                        isValid = parameters.Get<bool>("IsValid");
                        errorMsg = parameters.Get<string>("ErrorMsg");

                        if (isValid)
                        {
                            transcope.Complete();
                            errorMsg = "Success Insert Log";
                            result = true;
                        }
                        else
                        {
                            transcope.Dispose();
                            errorMsg = "Failed Insert Log, rollback transaction for Log";
                            result = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        transcope.Dispose();
                        errorMsg = "Failed Insert Log, " + ex.Message;
                        _logger.LogError(ex, "{Message}", ex.Message);
                        result = false;
                    }

                    return result;
                }
            }
            else
            {
                return false;
            }
        }

        public void LogFunction(LogHeader logH, string messageId, string method, string status, out string msgError, string username = null, string param0 = "", string param1 = "", string param2 = "", string param3 = "", string errMsg = "", string activity = null, string host = "")
        {
            LogDetail logDetail = new LogDetail();
            logH.LogDetail = new List<LogDetail>();
            logH.PROCESS_STATUS = (status == "Success") ? EnumHelper.GetDescription(ProcessStatus.FinishSuccessfully) :
                                  (status == "Failed") ? EnumHelper.GetDescription(ProcessStatus.FinishBusinessError)
                                                       : EnumHelper.GetDescription(ProcessStatus.FinishSystemError);
            logH.USER_ID = (username == null || username == "") ? "admin" : username;
            logH.END_DT = DateTime.Now;
            logDetail.ERROR_DT = DateTime.Now;
            logDetail.ERROR_LOC = method;

            MessageService msgService = new MessageService(Db);

            var message = msgService.GetMessagebyID(messageId);

            logDetail.MESSAGE_ID = message.MESSAGE_ID;
            if (string.IsNullOrEmpty(errMsg))
            {
                logDetail.ERROR_MESSAGE = message.MESSAGE_ID + " : " + string.Format(message.MESSAGE_TEXT, param0, param1, param2, param3);
            }
            else
            {
                logDetail.ERROR_MESSAGE = message.MESSAGE_ID + " : " + errMsg;
            }
            logDetail.MESSAGE_TYPE = message.MESSAGE_TYPE;
            msgError = string.Format(message.MESSAGE_TEXT, param0, param1, param2, param3);
            logH.LogDetail.Add(logDetail);
            CreateLog(logH);

            //Insert Data To New Log Standarization
            LogProcess(logH.USER_ID, logH.MODULE_ID, host, activity, status, errMsg);
        }

        public bool CreateLog(LogHeader data)
        {
            var log = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            bool _result = false;
            bool isValid = false;
            string message = "";
            using (TransactionScope transcope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    var parameters = new DynamicParameters();
                    string startDate = data.START_DT.ToString("yyyy-MM-dd HH:mm:ss.ffff");
                    string endDate = data.END_DT.ToString("yyyy-MM-dd HH:mm:ss.ffff");
                    //parameters.Add("@PROCESS_ID", data.PROCESS_ID);
                    parameters.Add("@START_DT", Convert.ToDateTime(startDate));
                    parameters.Add("@END_DT", Convert.ToDateTime(endDate));
                    parameters.Add("@MODULE_ID", data.MODULE_ID);
                    parameters.Add("@FUNCTION_ID", data.FUNCTION_ID);
                    parameters.Add("@PROCESS_STATUS", data.PROCESS_STATUS);
                    //parameters.Add("@USER_ID", SessionManager.Current);
                    parameters.Add("@USER_ID", "admin");
                    //parameters.Add("@READ_FLAG", data.READ_FLAG);
                    parameters.Add("@REMARK", data.REMARK);

                    DataTable dtLogDetail = DTlogDetail();
                    for (int i = 0; i < data.LogDetail.Count; i++)
                    {
                        dtLogDetail.Rows.Add(
                            0
                            , i + 1
                            //, data.LogDetail[i].ERROR_DT
                            , null
                            , data.LogDetail[i].ERROR_LOC
                            , data.LogDetail[i].MESSAGE_TYPE
                            , data.LogDetail[i].MESSAGE_ID
                            , data.LogDetail[i].ERROR_MESSAGE
                        );
                    }

                    var detailData = JsonConvert.SerializeObject(dtLogDetail);

                    parameters.Add("@UDT_LOG_D", dtLogDetail.AsTableValuedParameter("dbo.UDT_LOG_D"));
                    parameters.Add("@Detail", detailData);
                    parameters.Add("@IsValid", isValid, DbType.Boolean, ParameterDirection.Output);
                    parameters.Add("@MsgError", message, DbType.String, ParameterDirection.Output, 4000);


                    Db.Connection.Execute(
                            //"USP_CREATE_LOG",
                            "USP_LOG_SAVE",
                            parameters,
                            Db.Transaction,
                            commandType: System.Data.CommandType.StoredProcedure);

                    isValid = parameters.Get<bool>("IsValid");
                    message = parameters.Get<string>("MsgError");

                    if (isValid)
                    {
                        transcope.Complete();
                        log.Information("Success Insert Log");
                        _result = true;
                    }
                    else
                    {
                        transcope.Dispose();
                        log.Error("Failed Insert Log, rollback transaction for Log");
                        _result = false;
                    }

                    return _result;
                }
                catch (Exception ex)
                {
                    transcope.Dispose();
                    log.Error("Failed Insert Log, " + ex.Message);
                    return _result = false;
                }
            }
        }

        public bool CreateLogArchive(LogHeader data)
        {
            var log = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            bool _result = false;
            bool isValid = false;
            string message = "";
            ///string userLogin = SessionManager.Current;
            string userLogin = "admin";
            using (TransactionScope transcope = new TransactionScope())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@START_DT", data.START_DT);
                    parameters.Add("@END_DT", data.END_DT);
                    parameters.Add("@UserLogin", userLogin);
                    parameters.Add("@IsValid", isValid, DbType.Boolean, ParameterDirection.Output);
                    parameters.Add("@MsgError", message, DbType.Boolean, ParameterDirection.Output, 4000);

                    Db.Connection.Execute(
                            //"USP_ARCHIVE_CREATE_LOG",
                            "USP_ARCHIVE_LOG_SAVE",
                            parameters,
                            Db.Transaction,
                            commandType: System.Data.CommandType.StoredProcedure);

                    isValid = parameters.Get<bool>("IsValid");
                    message = parameters.Get<string>("MsgError");

                    if (isValid)
                    {
                        transcope.Complete();
                        log.Information("Success Insert Log");
                        _result = true;
                    }
                    else
                    {
                        transcope.Dispose();
                        log.Error("Failed Insert Log, rollback transaction for Log");
                        _result = false;
                    }

                    return _result;
                }
                catch (Exception ex)
                {
                    transcope.Dispose();
                    log.Error("Failed Insert Log, " + ex.Message);
                    return _result = false;
                }
            }
        }

        static DataTable DTlogDetail()
        {
            DataTable dt = new DataTable();


            dt.Columns.Add("PROCESS_ID", typeof(int));
            dt.Columns.Add("SEQ_NO", typeof(int));
            dt.Columns.Add("ERROR_DT", typeof(DateTime));
            dt.Columns.Add("ERROR_LOC", typeof(string));
            dt.Columns.Add("MESSAGE_TYPE", typeof(string));
            dt.Columns.Add("MESSAGE_ID", typeof(string));
            dt.Columns.Add("ERROR_MESSAGE", typeof(string));

            return dt;
        }

        public void LogSave(LogService logService, LogHeader logH, string status, string errLocMessage, string msgError)
        {
            LogDetail logDetail = new LogDetail();
            logH.LogDetail = new List<LogDetail>();
            if (status == "Success" && msgError == "")
            {
                logH.PROCESS_STATUS = "0";
                logDetail.ERROR_DT = DateTime.Now;
                logDetail.ERROR_LOC = errLocMessage;
                logDetail.MESSAGE_TYPE = "I";
                logDetail.MESSAGE_ID = "MTPS00016I"; // Saving data is completed successfully
                logDetail.ERROR_MESSAGE = "MTPS00016I : Saving data is completed successfully";
                logH.LogDetail.Add(logDetail);
                CreateLog(logH);
            }
            else
            {
                logH.PROCESS_STATUS = "1";
                logDetail.ERROR_DT = DateTime.Now;
                logDetail.ERROR_LOC = errLocMessage;
                logDetail.MESSAGE_TYPE = "E";
                logDetail.MESSAGE_ID = "MTPS00130E"; // Saving Data is Failed
                logDetail.ERROR_MESSAGE = "MTPS00130E : " + msgError;
                logH.LogDetail.Add(logDetail);
                CreateLog(logH);
            }
        }

        public void LogDelete(LogService logService, LogHeader logH, string status, string errLocMessage, string msgError)
        {
            LogDetail logDetail = new LogDetail();
            logH.LogDetail = new List<LogDetail>();
            if (status == "Success" && msgError == "")
            {
                logH.PROCESS_STATUS = "0";
                logDetail.ERROR_DT = DateTime.Now;
                logDetail.ERROR_LOC = errLocMessage;
                logDetail.MESSAGE_TYPE = "I";
                logDetail.MESSAGE_ID = "MTPS00020I"; // Data has been deleted successfully
                logDetail.ERROR_MESSAGE = "MTPS00020I : Data has been deleted successfully";
                logH.LogDetail.Add(logDetail);
                CreateLog(logH);
            }
            else
            {
                logH.PROCESS_STATUS = "1";
                logDetail.ERROR_DT = DateTime.Now;
                logDetail.ERROR_LOC = errLocMessage;
                logDetail.MESSAGE_TYPE = "E";
                logDetail.MESSAGE_ID = "MTPS00130E"; // Saving Data is Failed
                logDetail.ERROR_MESSAGE = "MTPS00130E : " + msgError;
                logH.LogDetail.Add(logDetail);
                CreateLog(logH);
            }
        }
    }
}
