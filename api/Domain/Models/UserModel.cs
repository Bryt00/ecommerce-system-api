using Microsoft.AspNetCore.Identity;

namespace ecommapi.Domain.Models
{
    public class User : IdentityUser
    {
        public int UserId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public required string Username { get; set; }
        public string Email { get; set; }
        public string? Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTkExpiry { get; set; }


    }
}