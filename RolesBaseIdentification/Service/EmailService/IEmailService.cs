namespace RolesBaseIdentification.Service.EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

}
