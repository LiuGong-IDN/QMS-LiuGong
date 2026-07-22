using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Infrastructure.Helper
{
    public class OtpService
    {
        private readonly IEmailService _emailService;

        public OtpService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendOtpAsync(string email, string otp)
        {
            string subject = "Your Password Reset Code";
            string body = $@"
            <p>Hello,</p>
            <p>Your password reset code is:</p>
            <h2>{otp}</h2>
            <p>This code will expire in 15 minutes.</p>";

            await _emailService.SendEmailAsync(email, subject, body);
        }
    }
}
