namespace ecommapi.Application.Configurations
{
    public class MailSettings
    {
        public string? FromMail { get; set; }
        public string FromName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Server { get; set; }
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
    }
}