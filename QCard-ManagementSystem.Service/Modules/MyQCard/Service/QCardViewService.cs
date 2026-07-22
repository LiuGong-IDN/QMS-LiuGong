using Modules.MyQCard.ViewModel;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Infrastructure.Session;

namespace QCard.ManagementSystem.Service.Modules.MyQCard.Service
{
    public class QCardViewService(IDbHelper db) : DbService(db)
    {
        public BasicDataResponse GetViewQCardByEmail()
        {
            var result = new BasicDataResponse();
            var email = SessionManager.Email;
            try
            {
                QCardView? qCard = Db.MyQCardRepository.GetViewQCardByEmail(email).FirstOrDefault();

                result.Data = qCard;
                result.Success = true;
                result.Message = string.Empty;
            }
            catch(Exception ex)
            {
                result.Success = false;
                result.Message = "Terjadi kesalahan sistem. " + ex.Message;
            }
            return result;
        }
    }
}