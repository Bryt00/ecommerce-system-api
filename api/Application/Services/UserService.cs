using System.Data.Entity;
using AutoMapper;
using ecommapi.Application.Contracts;
using ecommapi.Application.Interfaces;
using ecommapi.Domain.Models;
using ecommapi.Infrastructure.Data;
using ecommapi.Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;

namespace ecommapi.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        //private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUserServie _currentUserService;
        private readonly UserManager<User> _userManager;


        public UserService(ITokenService tokenService, ApplicationDbContext context, ILogger<UserService> logger, ICurrentUserServie currentUserService, UserManager<User> userManager, IMapper mapper)
        {
            _tokenService = tokenService;
            // _context = context;
            _logger = logger;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _mapper = mapper;
        }
        //user registration handler
        public async Task<UserResponse> RegisterUserAsync(UserRegisRequest regisRequest)
        {
            try
            {
                _logger.LogInformation("Registering a new user with username: {Username}", regisRequest.Username);
                var existingUser = await _userManager.FindByEmailAsync(regisRequest.Email);
                if (existingUser != null)
                {
                    _logger.LogError("Email {Email} is already in use.", regisRequest.Email);
                    throw new Exception("User with this email already exists.");
                }
                //getting an instance of the user model
                var user = _mapper.Map<User>(regisRequest);

                //creates a user model
                var result = await _userManager.CreateAsync(user, regisRequest.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(":", result.Errors.Select(e => e.Description));
                    _logger.LogError("User creation failed: {errors}", errors);
                }
                _logger.LogInformation("User created successfully");
                await _tokenService.GenerateToken(user);
                // await _userManager.Ge
                //returns a new user response
                return _mapper.Map<UserResponse>(user);
                //_userManager.AddToRoleAsync(user, "User").Wait();

            }
            catch (Exception e)
            {
                _logger.LogError($"Error registering user: {e.Message}");
                throw;
            }

        }

        //user login handler
        public async Task<UserResponse> LoginAsync(UserLoginRequest userLoginRequest)
        {
            try
            {
                if (userLoginRequest == null)
                {
                    _logger.LogError("Login request is null");
                    throw new ArgumentNullException(nameof(userLoginRequest));
                }
                var user = await _userManager.FindByEmailAsync(userLoginRequest.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, userLoginRequest.Password))
                {
                    _logger.LogError("Invalid email or password");
                    throw new Exception("Invalid email or password");

                }
                //generate access token
                var accessToken = await _tokenService.GenerateToken(user);
                //generate the refresh token
                var refreshToken = _tokenService.GenerateRefreshToken();

                user.RefreshToken = Helpers.HashToken(refreshToken);
                user.RefreshTkExpiry = DateTime.Now.AddDays(3); // Set refresh token expiry to 3 days

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = string.Join(":", result.Errors.Select(e => e.Description));
                    _logger.LogError("Updating user with refresh token failed: {errors}", errors);
                    throw new Exception("An error occurred while updating the user with the refresh token.");
                }
                var userResponse = _mapper.Map<User, UserResponse>(user);
                userResponse.AccessToken = accessToken;
                userResponse.RefreshToken = refreshToken;

                return userResponse;

            }
            catch (Exception e)
            {
                _logger.LogError($"Error during login: {e.Message}");
                throw;
            }
        }
        public async Task<UserResponse> GetUserByIdAsync(Guid id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    _logger.LogError("User with ID {UserId} not found", id);
                    throw new Exception("User not found");
                }
                return _mapper.Map<UserResponse>(user);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error retrieving user by ID: {e.Message}");
                throw;
            }
        }
        public async Task<CurrentUserResponse> GetCurrentUserAsync()
        {
            try
            {
                var user = await _userManager.FindByIdAsync(_currentUserService.GetUserId());
                if (user == null)
                {
                    _logger.LogError(" user not found");
                    throw new Exception("User not found");
                }
                return _mapper.Map<CurrentUserResponse>(user);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error retrieving current user: {e.Message}");
                throw;
            }
        }

        public async Task<UserResponse> UpdateUserAsync(Guid id, UpdateUserRequest updateUserRequest)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    _logger.LogError("User not found");
                    throw new Exception("user not found");
                }
                user.FirstName = updateUserRequest.FirstName;
                user.LastName = updateUserRequest.LastName;
                user.Email = updateUserRequest.Email;

                await _userManager.UpdateAsync(user);
                return _mapper.Map<UserResponse>(user);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error updating user: {e.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {id} not found ");
                    return false;
                }
                await _userManager.DeleteAsync(user);
                _logger.LogInformation($"User with ID {id} deleted successfully");
                return true;
            }
            catch (Exception)
            {
                _logger.LogError($"Error deleting user with ID {id}");
                return false;
            }
        }

        public Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }





        public async Task<CurrentUserResponse> RefreshTokenAsync(RefreshTkRequest refreshTkRequest)
        {
            try
            {
                var hashedRefreshToken = Helpers.HashToken(refreshTkRequest.RefreshToken);
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == hashedRefreshToken);

                if (user == null)
                {
                    _logger.LogError("Invalid refresh token");
                    throw new Exception("Invalid refresh token");
                }

                // validating refresh token expiry
                if (user.RefreshTkExpiry < DateTime.Now)
                {
                    _logger.LogError("Refresh token expired");
                    throw new Exception("Refresh token expired ");
                }
                //Generate new access token if refresh token is not expired
                var newAccessTk = await _tokenService.GenerateToken(user);
                _logger.LogInformation("Access token generated successfully");
                var currentUserResponse = _mapper.Map<CurrentUserResponse>(user);
                currentUserResponse.AccessToken = newAccessTk;
                return currentUserResponse;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error refreshing token: {e.Message}");
                throw;
            }
        }



        public async Task<RevokeRefreshTkResponse> RevokeRefreshTokenAsync(RefreshTkRequest refreshTkRequest)
        {
            _logger.LogInformation("Revoking refresh token");
            try
            {
                var hashedRefreshToken = Helpers.HashToken(refreshTkRequest.RefreshToken);

                //finding user using refresh token
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == hashedRefreshToken);
                if (user == null)
                {
                    _logger.LogError("Invalid refresh token");
                    throw new Exception("Invalid refresh token");
                }
                if (user.RefreshTkExpiry < DateTime.Now)
                {
                    _logger.LogWarning($"Refresh token expired for user ID: {user.Id}");
                    throw new Exception("Refresh token expired");
                }
                //remove refresh token
                user.RefreshToken = null;
                user.RefreshTkExpiry = null;

                //updating DB
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(":", result.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to update user:{errors}", errors);
                    //throw new Exception($"Failed to update user: {errors}");

                    return new RevokeRefreshTkResponse
                    {
                        Message = "Failed to revoke refresh token"
                    };


                }

                _logger.LogInformation("Refresh token successfully removed");
                return new RevokeRefreshTkResponse
                {
Message = "revoke success"
                };

            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to revoke refresh token: {e.Message}");
                throw;
            }
        }


    }
}