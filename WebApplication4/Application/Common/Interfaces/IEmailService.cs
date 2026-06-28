namespace WebApplication4.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlMessage);
    }
}
