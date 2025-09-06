namespace ecommapi.Application.Contracts
{

    public record ErrorResponse
    {
        public string? Title { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
   } 
}
    
