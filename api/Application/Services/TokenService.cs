using System.Security.Claims;
using System.Text;
using ecommapi.Application.Configurations;
using ecommapi.Application.Interfaces;
using ecommapi.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace ecommapi.Application.Services
{
    public class TokenService : ITokenService

    {
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        private readonly string? _validIssuer;
        private readonly string? _validAudience;
        private readonly double _expires;
        private readonly UserManager<User> _userManager;

        public TokenService(IConfiguration configuration, UserManager<User> userManager)
        {
            _userManager = userManager;
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key))
            {
                throw new InvalidOperationException("JWT secret key is not configured.");
            }
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            _validIssuer = jwtSettings.ValidIssuer;
            _validAudience = jwtSettings.ValidAudience;
            _expires = jwtSettings.Expires;
        }
        public async Task<string> GenerateToken(User user)
        {
            var signingCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var claims = await GetClaimsAsync(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private async Task<List<Claim>> GetClaimsAsync(User user)
        {
            var claims = new List<Claim>
            {
             new Claim(ClaimTypes.NameIdentifier, user.Id),
             new Claim(ClaimTypes.Name, user?.Username ?? string.Empty),
             new Claim(ClaimTypes.Email, user?.Email ?? string.Empty)
            };
            var roles = await _userManager.GetRolesAsync(user!);


            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            return claims;
        }
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {

            return new JwtSecurityToken(
                issuer: _validIssuer,
                audience: _validAudience,
                claims: claims,
               expires: DateTime.Now.AddMinutes(_expires),
                signingCredentials: signingCredentials
            );
        }
        public string GenerateRefreshToken()
        {
            var randomToken = new byte[64];
            using var rnGen = RandomNumberGenerator.Create();
            rnGen.GetBytes(randomToken);
            var refreshToken = Convert.ToBase64String(randomToken);
            return refreshToken; 
        }


    }
}