using Dapper;
using System.Transactions;
using TAM.WRH.Domain;
using TAM.WRH.Domain.Enums;
using TAM.WRH.Domain.Modules.Core.Model;
using TAM.WRH.Infrastructure.General;
using Function = TAM.WRH.Domain.Enums.Function;
using Message = TAM.WRH.Domain.Enums.Message;

namespace TAM.WRH.Service
{
    public class BatchService : DbService
    {
        public BatchService(IDbHelper db) : base(db)
        {

        }


        public List<BatchResponse> GetBatch()
        {

            var strsql = @"SELECT * FROM [dbo].[UDF_BATCH_GET]()";
            var dbdat = Db.Connection.Query<BatchResponse>(strsql).ToList();
            if (dbdat.Count > 0)
                return dbdat;

            return new List<BatchResponse>();
        }

        public List<BatchQueueResponse> GetBatchQueue()
        {

            var strsql = @"SELECT * FROM [dbo].[UDF_BATCH_QUEUE_GET]()";
            var dbdat = Db.Connection.Query<BatchQueueResponse>(strsql).ToList();
            if (dbdat.Count > 0)
                return dbdat;

            return new List<BatchQueueResponse>();
        }


        public bool BatchUpdateQueue(BatchQueueRequest data, out string msgerror)
        {
            using (DbHelper db = new DbHelper())
            {
                LogHeader logH = new LogHeader();
                var result = false;
                msgerror = "";
                logH.START_DT = DateTime.Now;
                logH.MODULE_ID = EnumHelper.GetDescription(Module.QueueLog);
                logH.FUNCTION_ID = EnumHelper.GetDescription(Function.BatchUpdate);
                logH.READ_FLAG = "N";
                logH.REMARK = "ExecuteAsyncWorker.BatchUpdateQueue";
                var method = "Batch Queue Update";
                var enumDesc = "";
                var errorLog = "";
                try
                {
                    var isValid = false;
                    var by = "admin";

                    var param = new DynamicParameters();
                    param.Add("@QUEUE_NO", data.QUEUE_NO);
                    param.Add("@STATUS", data.STATUS);
                    param.Add("@MESSAGE", data.MESSAGEERROR);
                    param.Add("@IsValid", isValid, System.Data.DbType.Boolean, System.Data.ParameterDirection.Output);
                    param.Add("@MsgError", msgerror, System.Data.DbType.String, System.Data.ParameterDirection.Output, 4000);

                    using (var escope = new TransactionScope())
                    {
                        Db.Connection.Execute("[dbo].[USP_BATCH_QUEUE_UPDATE]", param, Db.Transaction, commandType: System.Data.CommandType.StoredProcedure);

                        isValid = param.Get<bool>("IsValid");
                        errorLog = param.Get<string>("MsgError");
                        if (isValid)
                        {
                            escope.Complete();
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    if (isValid)
                    {
                        enumDesc = EnumHelper.GetDescription(Message.SaveSuccess);
                        LogService logService = new LogService(db);
                        //logService.LogFunction(logH, enumDesc, method, "Success", out msgerror);
                        logService.LogSave(logService, logH, "Success", method, msgerror);
                    }
                    else
                    {
                        enumDesc = EnumHelper.GetDescription(Message.SaveFailed);
                        LogService logService = new LogService(db);
                        //logService.LogFunction(logH, enumDesc, method, "Failed", out msgerror);
                        logService.LogSave(logService, logH, "Failed", method, msgerror);
                        msgerror = errorLog;
                    }

                    return result;
                }
                catch (Exception e)
                {
                    var msg = "";
                    if (e.InnerException != null)
                        msg = String.Concat(e.InnerException.Message, Environment.NewLine, e.StackTrace);
                    else
                        msg = String.Concat(e.Message, Environment.NewLine, e.StackTrace);

                    enumDesc = EnumHelper.GetDescription(Message.SaveFailed);
                    LogService logService = new LogService(db);
                    logService.LogFunction(logH, enumDesc, method, "Error", out msgerror);
                    return result = false;
                }
            }
        }

    }

}
