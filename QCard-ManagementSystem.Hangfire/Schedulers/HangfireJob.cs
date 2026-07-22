using Hangfire;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Infrastructure.Helper;
using QCard.ManagementSystem.Service.Modules.Core.Service;
using QCard.ManagementSystem.Service.Modules.Hangfire.Service;

namespace QCard.ManagementSystem.Hangfire.Schedulers
{
    public class HangfireJob : IHangfireJob
    {
        private readonly IServiceProvider _serviceProvider;
        public HangfireJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void RegisterSchedule() => RunService();

        public void RunService()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IDbHelper>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            HangfireJobService hangfireJobService = new(db, emailService);

            RecurringJob.AddOrUpdate(
                "JobTesting", () => hangfireJobService.TestingJob(), Cron.Monthly,
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Local }
            );

            RecurringJob.RemoveIfExists("JobSendEmailExpiryQCard");
            RecurringJob.AddOrUpdate(
                "JobSendEmailExpiryQCard", () => hangfireJobService.SendEmailExpiryQCard(), Cron.Monthly(1, 8),
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Local }
            );
        }
    }

    public interface IHangfireJob
    {
        void RegisterSchedule();
    }
}