using ecommapi.Domain.Models;

namespace ecommapi.Application.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
        string GenerateRefreshToken();
    }
}