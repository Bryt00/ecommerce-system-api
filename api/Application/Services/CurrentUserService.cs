using System.Security.Claims;
using ecommapi.Application.Contracts;
using ecommapi.Application.Interfaces;

namespace ecommapi.Application.Services
{
    public class CurrentUserService : ICurrentUserServie
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        //public CurrentUserService(){}
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string? GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }
    }
}