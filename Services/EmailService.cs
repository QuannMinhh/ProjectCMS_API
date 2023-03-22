using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using ProjectCMS.ViewModels;

namespace ProjectCMS.Services
{
    public class EmailService
    {
        private async Task SendEmailAsync(SendEmailModel email)
        {


            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("CMS Notification", "testapiweb123@gmail.com"));
            message.To.Add(new MailboxAddress("", email.ToEmail));
            message.Subject = email.Subject;


            var builder = new BodyBuilder();
            builder.HtmlBody = email.Body;
            message.Body = builder.ToMessageBody(); // Chuyển nội dung HTML thành nội dung email và gán cho đối tượng MimeMessage

            //Tạo một đối tượng SmtpClient để gửi email
            using (var client = new SmtpClient())
            {
                // Kết nối đến máy chủ email
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // Xác thực với máy chủ email bằng tài khoản và mật khẩu
                await client.AuthenticateAsync("testapiweb123@gmail.com", "");
                
                // Gửi email 
                await client.SendAsync(message);

                // Ngắt kết nối với máy chủ email
                await client.DisconnectAsync(true);
            }
            return;
        }
        //ungrowmjjlflqjfv
        public async Task NewIdeaNotify(string eventName, string submiter, string[] admin)
        {
            try
            {

                string body = "User " + submiter + " submitted an idea to the event " + eventName;
                foreach (var user in admin)
                {
                    SendEmailModel newEmail = new()
                    {
                        ToEmail = user,
                        Subject = "New Idea submited",
                        Body = body
                    };

                    await SendEmailAsync(newEmail);
                }
                return;
            }catch(Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
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
            return;
        }

        public async Task ForgotPassword(string newPass, string toEmail)
        {
            string body = "New password of your account is: " + newPass;
            SendEmailModel newEmail = new()
            {
                ToEmail = toEmail,
                Subject = "Password has been reset",
                Body = body
            };

            await SendEmailAsync(newEmail);
            return;
        }

    }
}
