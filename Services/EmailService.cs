using MailKit.Net.Smtp; // Thư viện để kết nối và gửi email
using MailKit.Security; // Thư viện cung cấp các tùy chọn bảo mật khi kết nối với máy chủ email
using MimeKit; // Thư viện cung cấp các lớp để tạo và sửa đổi nội dung email
using ProjectCMS.ViewModels;

namespace ProjectCMS.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(SendEmailModel email)
        {
            
            // Tạo một đối tượng MimeMessage để tạo email
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("CMS Notification", "testapiweb123@gmail.com")); // Thêm địa chỉ email người gửi
            message.To.Add(new MailboxAddress("", email.ToEmail)); // Thêm địa chỉ email người nhận
            message.Subject = email.Subject; // Đặt chủ đề email

            // Tạo đối tượng BodyBuilder để tạo nội dung email
            var builder = new BodyBuilder();
            builder.HtmlBody = email.Body; // Đặt nội dung HTML của email
            message.Body = builder.ToMessageBody(); // Chuyển nội dung HTML thành nội dung email và gán cho đối tượng MimeMessage

            // Tạo một đối tượng SmtpClient để gửi email
            using (var client = new SmtpClient())
            {
                // Kết nối đến máy chủ email
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // Xác thực với máy chủ email bằng tài khoản và mật khẩu
                await client.AuthenticateAsync("testapiweb123@gmail.com", "pnybsptghklraqbf");

                // Gửi email 
                await client.SendAsync(message);

                // Ngắt kết nối với máy chủ email
                await client.DisconnectAsync(true);
            }
        }
    }
}
