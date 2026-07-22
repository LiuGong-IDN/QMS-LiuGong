using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Service.Modules.Core.Service;

namespace QCard.ManagementSystem.Service.Modules.Scheduler.Service
{
    public class ControlBatchProcessService : DbService
    {
        public ControlBatchProcessService(IDbHelper db) : base(db)
        {
        }

        public BasicResponse Update(string function, string status)
        {
            string methodName = "Update";
            BasicResponse message = new BasicResponse();
            try
            {
                var result = Db.ControlBatchProcessRepository.Update(function, status);

                if (result > 0)
                {
                    message.Success = true;
                    message.Message = string.Empty;
                }
                else
                {
                    throw new Exception("Failed Update Data");
                }
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
                message.Success = false;
                message.Message = ex.Message;
            }

            return message;
        }
    }
}
