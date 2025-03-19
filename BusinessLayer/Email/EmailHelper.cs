using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BusinessLayer.Email
{
    public class EmailHelper
    {
        private readonly IConfiguration _config;
        public EmailHelper(IConfiguration config) 
        {
            _config = config;
        }
       public async Task<bool> SendPasswordResetEmailAsync(string email, string resetToken)
{
    try
    {
        // Read API Base URL from appsettings.json
        string apiBaseUrl = _config["AppSettings:ApiBaseUrl"];
        // Construct the Reset Password API URL
        string resetUrl = $"{apiBaseUrl}/api/auth/reset-password?token={resetToken}";

        string emailBody = $@"
        <html>
        <body>
        <h2>Password Reset Request</h2>
        <p>Hello,</p>
        <p>You requested to reset your password. Please use the link below to set a new password:</p>
        <p>
            <strong>Reset Password Link:</strong><br>
            <a href='{resetUrl}'>{resetUrl}</a>
        </p>
        <p>If you did not request this, please ignore this email.</p>
        <p>Thank you,<br>Team</p>
        </body>
        </html>";

        // Create email message
        MailMessage mail = new MailMessage
        {
            From = new MailAddress(_config["EmailSettings:SenderEmail"]),
            Subject = "Password Reset Request",
            Body = emailBody,
            IsBodyHtml = true
        };
        mail.To.Add(email);

        // Configure SMTP client
        using SmtpClient smtp = new SmtpClient(_config["EmailSettings:SmtpServer"])
        {
            Port = int.Parse(_config["EmailSettings:Port"]),
            Credentials = new System.Net.NetworkCredential(
                _config["EmailSettings:Username"],
                _config["EmailSettings:Password"]
            ),
            EnableSsl = bool.Parse(_config["EmailSettings:EnableSsl"])
        };

        // Send the email asynchronously
        await smtp.SendMailAsync(mail);

        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Email sending failed: {ex.Message}");
        return false;
    }
}

    }
}
