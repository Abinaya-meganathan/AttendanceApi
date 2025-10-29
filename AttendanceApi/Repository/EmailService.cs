using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace AttendanceApi.Repository
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        // ✅ Send simple email (existing method)
        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var mail = _config.GetSection("MailSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(mail["SenderName"], mail["SenderEmail"]));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(mail["SmtpServer"], int.Parse(mail["Port"]), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(mail["UserName"], mail["Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        // ✅ NEW: Send email with Excel attachment
        public async Task SendEmailWithAttachmentAsync(
            string toEmail,
            string subject,
            string htmlBody,
            byte[] attachmentData,
            string attachmentFileName)
        {
            var mail = _config.GetSection("MailSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(mail["SenderName"], mail["SenderEmail"]));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };

            // Add attachment
            builder.Attachments.Add(attachmentFileName, attachmentData, ContentType.Parse("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(mail["SmtpServer"], int.Parse(mail["Port"]), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(mail["UserName"], mail["Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}