namespace ecommapi.Application.Contracts
{
    public class UserRegisRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        //public DateTime CreatedAt { get; set; } 
    }
}