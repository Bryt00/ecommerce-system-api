namespace ecommapi.Application.Configurations
{
    public class JwtSettings
    {
        public string? Key { set; get; }
        public string ValidIssuer { set; get; }
        public string ValidAudience { set; get; }
        public double Expires{ set; get; }
        
    }
}