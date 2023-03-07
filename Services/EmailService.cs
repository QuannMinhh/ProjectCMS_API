using MailKit.Net.Smtp; 
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using ProjectCMS.Data;
using ProjectCMS.Models;
using ProjectCMS.ViewModels;
using static Duende.IdentityServer.Models.IdentityResources;

namespace ProjectCMS.Services
{
    public class EmailService
    {
        private readonly ApplicationDbContext _dbContext;
        public EmailService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private async Task SendEmailAsync(SendEmailModel email)
        {


            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("CMS Notification", "testapiweb123@gmail.com"));
            message.To.Add(new MailboxAddress("", email.ToEmail));
            message.Subject = email.Subject;


            var builder = new BodyBuilder();
            builder.HtmlBody = email.Body;
            message.Body = builder.ToMessageBody(); // Chuyển nội dung HTML thành nội dung email và gán cho đối tượng MimeMessage

            // Tạo một đối tượng SmtpClient để gửi email
            using (var client = new SmtpClient())
            {
                // Kết nối đến máy chủ email
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // Xác thực với máy chủ email bằng tài khoản và mật khẩu
                await client.AuthenticateAsync("testapiweb123@gmail.com", "wanoopyitnbbheqi");

                // Gửi email 
                await client.SendAsync(message);

                // Ngắt kết nối với máy chủ email
                await client.DisconnectAsync(true);
            }
        }

        public async Task NewIdeaNotify(string eventName, string submiter)
        {
            List<User> admin = await _dbContext._users.Where(u => u.Role == "Admin").ToListAsync();

            string body ="User " + submiter + " submitted an idea to the event " + eventName;
            foreach (var user in admin)
            {
                SendEmailModel newEmail = new()
                {
                    ToEmail = user.Email,
                    Subject = "New Idea submited",
                    Body = body
                };

                await SendEmailAsync(newEmail);
            }
        }

        public async Task NewCommentNotify(string submiter, string toEmail)
        {
            string body = "User " + submiter + " commented on your idea";
            SendEmailModel newEmail = new()
            {
                ToEmail = toEmail,
                Subject = "New Idea submited",
                Body = body
            };

            await SendEmailAsync(newEmail);
        }

    }
}
