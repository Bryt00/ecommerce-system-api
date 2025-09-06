using ecommapi.Application.Contracts;

namespace ecommapi.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> RegisterUserAsync(UserRegisRequest regisRequest);
        Task<UserResponse> LoginAsync(UserLoginRequest userLoginRequest);
        Task<CurrentUserResponse> GetCurrentUserAsync();
        Task<UserResponse> GetUserByIdAsync(Guid id);
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<UserResponse> UpdateUserAsync(Guid id, UpdateUserRequest updateUserRequest);
        Task<bool> DeleteUserAsync(Guid id);
        Task<CurrentUserResponse> RefreshTokenAsync(RefreshTkRequest refreshTkRequest);
        Task<RevokeRefreshTkResponse> RevokeRefreshTokenAsync(RefreshTkRequest refreshTkRequest);
    }
}