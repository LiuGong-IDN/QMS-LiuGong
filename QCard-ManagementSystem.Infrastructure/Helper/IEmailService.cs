using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using Dapper;
using Microsoft.Extensions.Options;
using Modules.MyMachines.Model;
using Modules.QCard.Model;
using QCard.ManagementSystem.Infrastructure.Helper;
using QCard.ManagementSystem.Infrastructure.Helper.Model;

    namespace QCard.ManagementSystem.Infrastructure.Helper
    {
        public interface IEmailService
        {
            Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
            Task SendEmailWithInlineImageAsync(string to, string subject, string body, bool isHtml = true, Dictionary<string, byte[]>? inlineAttachments = null);
            Task SendBulkExpiryNotificationAsync(IEnumerable<QCardExpiryInfo> cardList);
            Task SendBulkTaskApprovalNotificationAsync(List<ApprovalEmail> emailList, string serialNo, string approverName, long _userId, string cardNo, string categoryClaim, string customer);
            Task SendBulkMachineNotificationAsync(List<PDCEmail> emailList, string serialNo, string pdcName, long _userId, string claimType, string claimNo, string companyName);
            Task SendBulkClaimUpdateNotificationAsync(List<PDCEmail> emailList, string serialNo, string pdcName, long _userId, string claimType, string claimNo, string companyName);
        }

        public class SmtpEmailService(IOptions<SmtpSettings> options) : IEmailService
        {
            private readonly SmtpSettings _settings = options.Value;

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
            {
                var settings = _settings;
                using var message = new MailMessage
                {
                    From = new MailAddress(_settings.From, _settings.DisplayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                message.To.Add(new MailAddress(to));

                using var client = new SmtpClient(_settings.Host, _settings.Port)
                {
                    Credentials = new NetworkCredential(_settings.User, _settings.Password),
                    EnableSsl = _settings.EnableSsl
                };

                await client.SendMailAsync(message);
            }

            public async Task SendEmailWithInlineImageAsync(string to, string subject, string body, bool isHtml = true, Dictionary<string, byte[]>? inlineAttachments = null)
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(_settings.From, _settings.DisplayName),
                    Subject = subject,
                    IsBodyHtml = isHtml
                };

                message.To.Add(new MailAddress(to));

                if (isHtml && inlineAttachments != null && inlineAttachments.Count != 0)
                {
                    // Create AlternateView untuk HTML dengan LinkedResource
                    var htmlView = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);

                    foreach (var kv in inlineAttachments)
                    {
                        var contentId = kv.Key;
                        var imageData = kv.Value;

                        string mediaType = GetMediaTypeFromContentId(contentId);

                        var ms = new MemoryStream(imageData);
                        var resource = new LinkedResource(ms, mediaType)
                        {
                            ContentId = contentId,
                            TransferEncoding = TransferEncoding.Base64
                        };

                        htmlView.LinkedResources.Add(resource);
                    }

                    message.AlternateViews.Add(htmlView);
                }
                else
                {
                    message.Body = body;
                }

                using var client = new SmtpClient(_settings.Host, _settings.Port)
                {
                    Credentials = new NetworkCredential(_settings.User, _settings.Password),
                    EnableSsl = _settings.EnableSsl
                };

                await client.SendMailAsync(message);
            }
            private static string GetMediaTypeFromContentId(string contentId)
            {
                var ext = Path.GetExtension(contentId).ToLowerInvariant();

                return ext switch
                {
                    ".png" => "image/png",   // Tidak ada di enum, jadi pakai string langsung
                    ".jpg" => MediaTypeNames.Image.Jpeg,
                    ".jpeg" => MediaTypeNames.Image.Jpeg,
                    ".gif" => MediaTypeNames.Image.Gif,
                    _ => MediaTypeNames.Application.Octet
                };
            }

            public async Task SendBulkExpiryNotificationAsync(IEnumerable<QCardExpiryInfo> cardList)
            {
                if (cardList == null || !cardList.Any())
                    return;

                foreach (var card in cardList)
                {
                    if (string.IsNullOrWhiteSpace(card.EMAIL))
                    {
                        Console.WriteLine($"⚠️ Skip: Email not found for CardNo {card.CARD_NO} ({card.COMPANY_NAME})");
                        continue;
                    }

                    string subject = $"Q-Card Expiry Reminder - {card.COMPANY_NAME}";
                    string body = $@"
                        <p>Dear {card.COMPANY_NAME},</p>
                        <p>This is a friendly reminder that your Q-Card <b>{card.CARD_NO}</b> will expire on <b>{card.EXPIRY_DATE:dd MMMM yyyy}</b>.</p>
                        <p>Please Topup your Q-Card before it expires by purchasing units to avoid any service interruption.</p>
                        <br/>
                        <p>Thank you,<br/>Q-Card Management System</p>
                    ";

                    try
                    {
                        await SendEmailAsync(card.EMAIL, subject, body, true);
                        Console.WriteLine($"✅ Email sent to {card.EMAIL} for {card.COMPANY_NAME}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Failed to send email to {card.EMAIL}: {ex.Message}");
                    }
                }
            }

        public async Task SendBulkMachineNotificationAsync(List<PDCEmail> emailList, string serialNo, string pdcName, long _userId, string claimType, string claimNo, string companyName)
        {
            if (emailList == null || !emailList.Any())
            {
                return;
            }

            foreach (var pdcEmail in emailList)
            {

                string subject = $"Claim Notification - {claimNo}";
                string body = $@"
                    <p>Dear <strong>{pdcName}</strong>,</p>
                    <p>This is a notification regarding Claim:</p>
                    <ul>
                        <li><strong>Claim Number:</strong> {claimNo}</li>
                        <li><strong>Serial Number:</strong> {serialNo}</li>
                        <li><strong>Category Claim:</strong> {claimType}</li>
                        <li><strong>PDC:</strong> {pdcName}</li>
                        <li><strong>Customer:</strong> {companyName}</li>
                    </ul>
                    <p>You can check the Claim status on your Dashboard.</p>
                    <br/>
                    <p>Thank you,<br/>Q-Card Management System</p>
                ";

                try
                {
                    await SendEmailAsync(pdcEmail.EMAIL, subject, body, true);
                    Console.WriteLine($"✅ Email sent to {pdcEmail.EMAIL} for machine {serialNo}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to send email to {pdcEmail.EMAIL}: {ex.Message}");
                }
            }
        }

        public async Task SendBulkClaimUpdateNotificationAsync(List<PDCEmail> emailList, string serialNo, string pdcName, long _userId, string claimType, string claimNo, string companyName)
        {
            if (emailList == null || !emailList.Any())
            {
                return;
            }

            foreach (var pdcEmail in emailList)
            {

                string subject = $"Claim Update Notification - {claimNo}";
                string body = $@"
                    <p>Dear <strong>{pdcName}</strong>,</p>
                    <p>The following claim has been updated:</p>
                    <ul>
                        <li><strong>Claim Number:</strong> {claimNo}</li>
                        <li><strong>Serial Number:</strong> {serialNo}</li>
                        <li><strong>Category Claim:</strong> {claimType}</li>
                        <li><strong>PDC:</strong> {pdcName}</li>
                        <li><strong>Customer:</strong> {companyName}</li>
                    </ul>
                    <p>Please review the latest status on your Q-Card Dashboard.</p>
                    <br/>
                    <p>Thank you,<br/>Q-Card Management System</p>
                ";

                try
                {
                    await SendEmailAsync(pdcEmail.EMAIL, subject, body, true);
                    Console.WriteLine($"✅ Email sent to {pdcEmail.EMAIL} for machine {serialNo}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to send email to {pdcEmail.EMAIL}: {ex.Message}");
                }
            }
        }
        public async Task SendBulkTaskApprovalNotificationAsync(List<ApprovalEmail> emailList, string serialNo, string approverName, long _userId, string cardNo, string categoryClaim, string customer)
        {
            if (emailList == null || !emailList.Any())
            {
                return;
            }

            foreach (var approvalEmail in emailList)
            {
                string formattedCardNo = Regex.Replace(cardNo, ".{4}", "$0-").TrimEnd('-');
                string subject = $"Top Up Notification - {serialNo}";
                string body = $@"
                    <p>Dear <strong>{approverName}</strong>,</p>
                    <p>This is a notification regarding Top Up submission:</p>
                    <ul>
                        <li><strong>Q-Card Number:</strong> {formattedCardNo}</li>
                        <li><strong>Serial Number:</strong> {serialNo}</li>
                        <li><strong>Category Claim:</strong> {categoryClaim}</li>
                        <li><strong>Customer:</strong> {customer}</li>
                    </ul>
                    <p>You can verify the Top Up submission in the Task Approval menu.</p>
                    <br/>
                    <p>Thank you,<br/>Q-Card Management System</p>
                ";

                try
                {
                    await SendEmailAsync(approvalEmail.EMAIL, subject, body, true);
                    Console.WriteLine($"✅ Email sent to {approvalEmail.EMAIL} for machine {serialNo}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to send email to {approvalEmail.EMAIL}: {ex.Message}");
                }
            }
        }
    }
}