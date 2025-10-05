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

        public EmailNotificationService(
            IOptions<EmailSettings> options,
            ILogger<EmailNotificationService> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        // 🟢 Gửi khi có booking mới
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
            }
        }

        // 🟢 Gửi khi teacher confirm
        public async Task SendBookingConfirmedAsync(BookingEmailDto dto)
        {
            try
            {
                var subject = $"[Co&Learn] Booking confirmed - {dto.TeacherName}";
                var body = $@"
                    <html>
                        <body style='font-family: Arial'>
                            <h2>Your booking has been confirmed!</h2>
                            <p><strong>Student:</strong> {dto.StudentName} ({dto.StudentEmail})</p>
                            <p><strong>Teacher:</strong> {dto.TeacherName} ({dto.TeacherEmail})</p>
                            <p><strong>Booked at:</strong> {dto.CreateAt:dd/MM/yyyy HH:mm}</p>
                            <p><b>Time:</b> {dto.StartTime:HH:mm dd/MM/yyyy} - {dto.EndTime:HH:mm dd/MM/yyyy}</p>
                            <p><b>Notes:</b> {dto.Notes}</p>
                            <hr/>
                            <p>Please complete the payement in 10 minutes! We look forward to seeing you in class!</p>
                        </body>
                    </html>";
                await SendEmailAsync(dto.ParentEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send booking confirmed email for booking {BookingId}", dto.BookingId);
            }
        }

        // 🟢 Gửi khi teacher decline
        public async Task SendBookingDeclineAsync(BookingEmailDto dto)
        {
            try
            {
                var subject = $"[Co&Learn] Booking declined - {dto.TeacherName}";
                var body = $@"
                    <html>
                        <body style='font-family: Arial'>
                            <h2>Your booking has been declined!</h2>
                            <p><strong>Teacher:</strong> {dto.TeacherName} ({dto.TeacherEmail})</p>
                            <p><strong>Student:</strong> {dto.StudentName} ({dto.StudentEmail})</p>

                            <p><strong>Booked at:</strong> {dto.CreateAt:dd/MM/yyyy HH:mm}</p>
                            <p><b>Time:</b> {dto.StartTime:HH:mm dd/MM/yyyy} - {dto.EndTime:HH:mm dd/MM/yyyy}</p>
                            <p><b>Notes:</b> {dto.Notes}</p>
                            <hr/>
                            <p>Please try to book another slot with the same or different teacher. We look forward to seeing you in class!</p>
                        </body>
                    </html>";
                await SendEmailAsync(dto.ParentEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send booking decline email for booking {BookingId}", dto.BookingId);
            }
        }

        // 🟢 Reminder trước giờ học
        public async Task SendReminderAsync(BookingEmailDto dto)
        {
            try
            {
                var subject = $"[Co&Learn] Reminder - upcoming class with {dto.TeacherName}";
                var body = BuildReminderHtml(dto);
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

        // 🧱 Hàm gửi email chung
        private async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpServer, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        // HTML Templates
        private string BuildBookingCreatedHtml(BookingEmailDto dto)
        {
            return $@"
                <html>
                    <body style='font-family: Arial'>
                        <h2>New booking received</h2>
                        <p><strong>Student:</strong> {dto.StudentName} ({dto.StudentEmail})</p>
                        <p><strong>Parent:</strong> {dto.ParentName} ({dto.ParentEmail})</p>
                        <p><strong>Teacher:</strong> {dto.TeacherName} ({dto.TeacherEmail})</p>
                        <p><strong>Booked at:</strong> {dto.CreateAt:dd/MM/yyyy HH:mm}</p>
                        <p><strong>Time:</strong> {dto.StartTime:HH:mm dd/MM/yyyy} - {dto.EndTime:HH:mm dd/MM/yyyy}</p>
                        <p><strong>Notes:</strong> {dto.Notes}</p>
                        <hr/>
                        <p>Please check your teacher dashboard for details.</p>
                    </body>
                </html>";
        }

        private string BuildReminderHtml(BookingEmailDto dto)
        {
            return $@"
                <html>
                    <body style='font-family: Arial'>
                        <h2>Class reminder</h2>
                        <p><strong>Course:</strong> {dto.BookingId}</p>
                        <p><strong>Teacher:</strong> {dto.TeacherName}</p>
                        <p><strong>Student:</strong> {dto.StudentName}</p>
                        <p><strong>Starts at:</strong> {dto.StartTime:dd/MM/yyyy HH:mm}</p>
                        <hr/>
                        <p>Get ready for your session!</p>
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
