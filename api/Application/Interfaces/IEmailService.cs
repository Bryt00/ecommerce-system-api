using ecommapi.Domain.Models;

namespace ecommapi.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailDto request);
    }
}