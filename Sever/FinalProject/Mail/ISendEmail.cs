
namespace FinalProject.Mail
{
    public interface ISendEmail
    {
        Task SendEmailAsync(string email, string subject, string bodyContent);
    }
}
