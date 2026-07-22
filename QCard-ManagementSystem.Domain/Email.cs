using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain
{
    public class Email
    {
        protected string _from;
        protected string _username;
        protected string _password;
        protected string _domain;
        protected string _host;
        protected int _port;

        public Email()
        {

        }

        public Email(string from, string username, string password, string domain, string host, int port)
        {
            this._from = from;
            this._username = username;
            this._password = password;
            this._domain = domain;
            this._host = host;
            this._port = port;
        }

        public string From
        {
            set { _from = value; }
        }

        public Email(string username, string password, string domain, string host, int port)
        {
            this._from = "";
            this._username = username;
            this._password = password;
            this._domain = domain;
            this._host = host;
            this._port = port;
        }

        public Email(string from, string host, int port)
        {
            this._from = from;
            this._username = "";
            this._password = "";
            this._domain = "";
            this._host = host;
            this._port = port;
        }

        public void SendEmail(string to, string subject, string messageBody)
        {
            try
            {
                using (var client = SetSmtp())
                {
                    MailMessage message = new MailMessage(_from, to, subject, messageBody);
                    message.IsBodyHtml = true;

                    client.Send(message);
                }
            }
            catch
            {
                throw;
            }
        }

        public void SendEmailDelay(string to, string subject, string messageBody)
        {
            try
            {
                using (var client = SetSmtp())
                {
                    MailMessage message = new MailMessage(_from, to, subject, messageBody);
                    message.IsBodyHtml = true;
                    client.EnableSsl = true;
                    client.Send(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.Message);
                throw;
            }
        }

        public void SendEmail(string to, string cc, string subject, string messageBody)
        {
            try
            {
                using (var client = SetSmtp())
                {
                    MailMessage message = new MailMessage(_from, to, subject, messageBody);
                    message.CC.Add(new MailAddress(cc));
                    message.IsBodyHtml = true;

                    client.Send(message);
                }
            }
            catch
            {
                throw;
            }
        }

        public void SendEmail(string to, List<string> cc, string subject, string messageBody)
        {
            try
            {
                using (var client = SetSmtp())
                {
                    MailMessage message = new MailMessage(_from, to, subject, messageBody);
                    foreach (var item in cc) message.CC.Add(new MailAddress(item));
                    message.IsBodyHtml = true;

                    client.Send(message);
                }
            }
            catch
            {
                throw;
            }
        }

        public void SendEmailWithAttachment(string to, List<string> cc, string subject, string messageBody, List<Attachment> attachmentList)
        {
            try
            {
                using (var client = SetSmtp())
                {
                    MailMessage message = new MailMessage(_from, to, subject, messageBody);
                    foreach (var item in cc) message.CC.Add(new MailAddress(item));
                    message.IsBodyHtml = true;
                    foreach (var attachment in attachmentList) message.Attachments.Add(attachment);

                    client.Send(message);
                }
            }
            catch
            {
                throw;
            }
        }

        public void SendEmailWithAttachment(List<string> to, List<string> cc, string subject, string messageBody, List<Attachment> attachmentList)
        {
            try
            {
                using (var client = SetSmtp())
                {
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(_from);
                    message.Subject = subject;
                    message.Body = messageBody;
                    message.IsBodyHtml = true;

                    foreach (var rec in to) message.To.Add(new MailAddress(rec));
                    foreach (var item in cc) message.CC.Add(new MailAddress(item));
                    foreach (var attachment in attachmentList) message.Attachments.Add(attachment);

                    client.Send(message);
                }
            }
            catch
            {
                throw;
            }
        }

        public void SendEmailWithAttachment(List<string> to, string subject, string messageBody, Attachment attachment)
        {
            try
            {
                using (var client = SetSmtp())
                {
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(_from);
                    message.Subject = subject;
                    message.Body = messageBody;
                    message.IsBodyHtml = true;
                    message.Attachments.Add(attachment);

                    foreach (var rec in to) message.To.Add(new MailAddress(rec));

                    client.Send(message);
                }
            }
            catch
            {
                throw;
            }
        }

        public void SendEmail(List<string> to, string subject, string messageBody)
        {
            try
            {
                using (var client = SetSmtp())
                {
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(_from);
                    message.Subject = subject;
                    message.Body = messageBody;
                    message.IsBodyHtml = true;

                    foreach (var rec in to) message.To.Add(new MailAddress(rec));

                    client.Send(message);
                }
            }
            catch
            {
                throw;
            }
        }

        public void SendEmail(List<string> to, List<string> cc, string subject, string messageBody)
        {
            try
            {
                using (var client = SetSmtp())
                {
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(_from);
                    message.Subject = subject;
                    message.Body = messageBody;
                    message.IsBodyHtml = true;

                    foreach (var rec in to) message.To.Add(new MailAddress(rec));
                    foreach (var item in cc) message.CC.Add(new MailAddress(item));

                    client.Send(message);
                }
            }
            catch
            {
                throw;
            }
        }

        private SmtpClient SetSmtp()
        {
            SmtpClient client = new SmtpClient();
            try
            {
                client.Host = _host;

                if (_port != 0)
                    client.Port = _port;

                if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
                {
                    NetworkCredential credential = new NetworkCredential(_username, _password);
                    if (!string.IsNullOrEmpty(_domain)) credential.Domain = _domain;
                    client.UseDefaultCredentials = false;
                    client.Credentials = credential;

                    if (string.IsNullOrEmpty(_domain))
                    {
                        client.EnableSsl = true;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.Message);
                throw;
            }
            return client;
        }
    }
}
