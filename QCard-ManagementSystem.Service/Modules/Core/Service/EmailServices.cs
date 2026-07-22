using System.Net.Mail;
using QCard.ManagementSystem.Domain;

namespace QCard.ManagementSystem.Service.Modules.Core.Service
{
    public class EmailService : DbService
    {
        private Email email;

        public EmailService(IDbHelper db) : base(db)
        {
            email = SetEmail();
        }

        private Email SetEmail()
        {
            var username = new ConfigService(Db).GetConfigByIdAndType("EMAIL", "EMAIL_USERNAME").SYSTEM_VALUE_TEXT;
            var password = new ConfigService(Db).GetConfigByIdAndType("EMAIL", "EMAIL_PASSWORD").SYSTEM_VALUE_TEXT;
            var domain = new ConfigService(Db).GetConfigByIdAndType("EMAIL", "EMAIL_DOMAIN").SYSTEM_VALUE_TEXT;
            var host = new ConfigService(Db).GetConfigByIdAndType("EMAIL", "EMAIL_HOST").SYSTEM_VALUE_TEXT;
            var port = new ConfigService(Db).GetConfigByIdAndType("EMAIL", "EMAIL_PORT").SYSTEM_VALUE_TEXT;
            var from = new ConfigService(Db).GetConfigByIdAndType("EMAIL", "EMAIL_FROM").SYSTEM_VALUE_TEXT;

            return new Email(from, username, password, domain, host, int.Parse(port));
        }

        public void SendEmailScheduler(string receiver, string subject, string body, Stream stream)
        {
            var methodName = "SendEmailScheduler";
            try
            {
                List<string> receiverList = receiver.Split(';').ToList();
                Attachment attachment = new Attachment(stream, GetFileNameFromStream(stream));

                email.SendEmailWithAttachment(receiverList, subject, body, attachment);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
        }

        public string GetFileNameFromStream(Stream stream)
        {
            string result = string.Empty;

            if (stream is FileStream fileStream)
            {
                result = Path.GetFileName(fileStream.Name);
            }

            return result;
        }
    }
}
