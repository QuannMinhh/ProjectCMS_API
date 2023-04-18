using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using ProjectCMS.ViewModels;

namespace ProjectCMS.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;   
        }

        private async Task SendEmailAsync(SendEmailModel email)
        {
            var subject = _config.GetSection("Body").GetValue<string>("Subject");
            var mail = _config.GetSection("Email").GetValue<string>("Account");
            var key = _config.GetSection("Body").GetValue<string>("string");
            var server = _config.GetSection("Email").GetValue<string>("serverMail");
            var port = _config.GetSection("Email").GetValue<int>("port");
            //create a new MimeMessage object and config email's sender
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(subject, mail));
            message.To.Add(new MailboxAddress("", email.ToEmail));
            message.Subject = email.Subject;
            //Create a body of email
            var builder = new BodyBuilder();
            builder.HtmlBody = email.Body;
            message.Body = builder.ToMessageBody();
            //Create a SmtpClient object to send email
            using (var client = new SmtpClient())
            {
                // Connect to email server. In here is
                // smtp server of google and port is 587
                await client.ConnectAsync(server, port, SecureSocketOptions.StartTls);
                // Authenticate with email server
                // with account and password in appsettings.json file
                await client.AuthenticateAsync(mail, key);
                // Send email
                await client.SendAsync(message);
                // Disconnect from the email server
                await client.DisconnectAsync(true);
            }
            return;
        }
        public async Task NewIdeaNotify(string eventName, string submiter, string[] admin)
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
        }

        public async Task NewCommentNotify(string submiter, 
                                                string toEmail, 
                                        string ideaName, 
                                                 string content)
        {
            string body = "User " + submiter + "has comment: "+ 
                                content +" . On your idea " + ideaName;
            SendEmailModel newEmail = new()
            {
                ToEmail = toEmail,
                Subject = "New Comment on your idea",
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
