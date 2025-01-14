using System.Net;
using System.Net.Mail;
using System.Text;

namespace FinalProject.Mail
{
    public class SendEmail : ISendEmail
    {

        public Task SendEmailAsync(string email, string subject, string bodyContent)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("nguyenxuanan19082003@gmail.com", "zueu hzqc hsgw nxph")
            };

            var emailMessage = new StringBuilder();
            emailMessage.AppendLine("<html>");
            emailMessage.AppendLine("<body>");
            emailMessage.AppendLine($"<p>Dear {email},</p>");
            emailMessage.AppendLine($"<p>{bodyContent}</p>");
            emailMessage.AppendLine("<br>");
            emailMessage.AppendLine("<p>Best regards,</p>");
            emailMessage.AppendLine("<p><strong>RoomRental Team</strong></p>");
            emailMessage.AppendLine("</body>");
            emailMessage.AppendLine("</html>");

            string message = emailMessage.ToString();

            // In nội dung email ra console trước khi gửi
            Console.WriteLine("=== Email Content ===");
            Console.WriteLine(message);
            Console.WriteLine("=====================");

            return client.SendMailAsync(
                new MailMessage(from: "nguyenxuanan19082003@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
