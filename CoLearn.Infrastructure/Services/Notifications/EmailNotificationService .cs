// CoLearn.Infrastructure/Notifications/EmailNotificationService.cs
using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure.Notifications
{
    public class EmailNotificationService : INotificationService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(IOptions<EmailSettings> options, ILogger<EmailNotificationService> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SendBookingCreatedAsync(BookingEmailDto dto)
        {
            try
            {
                var subject = $"[Co&Learn] New booking #{dto.BookingId} - {dto.StudentName}";
                var body = BuildBookingCreatedHtml(dto);
                await SendEmailAsync(dto.TeacherEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send booking created email for booking {BookingId}", dto.BookingId);
                // Optionally persist failed notification for retry
            }
        }

        public async Task SendReminderAsync(BookingEmailDto dto)
        {
            try
            {
                var subject = $"[Co&Learn] Reminder - upcoming class {dto.StudentName}";
                var body = BuildReminderHtml(dto);
                // send to student + teacher
                await Task.WhenAll(
                    SendEmailAsync(dto.StudentEmail, subject, body),
                    SendEmailAsync(dto.TeacherEmail, subject, body)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send reminder for booking {BookingId}", dto.BookingId);
            }
        }

        private async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = htmlBody };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpServer, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        private string BuildBookingCreatedHtml(BookingEmailDto dto)
        {
            string start = dto.StartTime != null
                ? dto.StartTime.ToString("dd/MM/yyyy HH:mm")
                : "N/A";
            string end = dto.EndTime != null
                ? dto.EndTime.ToString("HH:mm")
                : "";

            return $@"
        <html>
          <body style='font-family: Arial'>
            <h2>New booking received</h2>
            <p><strong>Student:</strong> {dto.StudentName} ({dto.StudentEmail})</p>
            <p><strong>Booked at:</strong> {dto.CreateAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm")}</p>
            <p><strong>Notes:</strong> {dto.Notes}</p>
            <hr/>
            <p>Open your dashboard to manage this booking.</p>
          </body>
        </html>";
        }

        private string BuildReminderHtml(BookingEmailDto dto)
        {
            string start = dto.StartTime != null
                ? dto.StartTime.ToString("dd/MM/yyyy HH:mm")
                : "N/A";

            return $@"
        <html>
          <body style='font-family: Arial'>
            <h2>Class reminder</h2>
            <p><strong>Course:</strong> {dto.BookingId}</p>
            <p><strong>Teacher:</strong> {dto.TeacherName}</p>
            <p><strong>Student:</strong> {dto.StudentName}</p>
            <p><strong>Starts at:</strong> {start}</p>
            <hr/>
            <p>Please be ready for the class.</p>
          </body>
        </html>";
        }

    }
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
