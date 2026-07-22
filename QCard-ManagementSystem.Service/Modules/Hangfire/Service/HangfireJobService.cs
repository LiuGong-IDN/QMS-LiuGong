using Microsoft.AspNetCore.Hosting;
using Modules.QCard.Model;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Infrastructure.Helper;
using QCard.ManagementSystem.Service.Modules.Core.Service;

namespace QCard.ManagementSystem.Service.Modules.Hangfire.Service
{
    public class HangfireJobService : DbServiceBase
    {
        private readonly IEmailService _emailService;
        public HangfireJobService(IDbHelper db, IEmailService emailService) : base(db)
        {
            _emailService = emailService;
        }

        public void TestingJob()
        {
            Console.WriteLine($"[{DateTime.Now}] Testing job executed successfully!");
        }
        public async Task SendEmailExpiryQCard()
        {
            string methodName = "SendEmailExpiryQCard";
            try
            {
                List<QCardExpiryInfo> qCardExpiryInfo = Db.QCardListRepository.GetQCardExpiryInfo();

                if (qCardExpiryInfo.Count == 0)
                {
                    Console.WriteLine("No QCard expiring within 2 months.");
                    return;
                }

                await _emailService.SendBulkExpiryNotificationAsync(qCardExpiryInfo);


                Console.WriteLine($"Sent {qCardExpiryInfo.Count} expiry notification emails.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
