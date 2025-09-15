namespace AGAMinigameApi.Models
{
    public class Agent
    {
        public short Id { get; set; } 
        public string Code { get; set; } = ""; 
        public string Name { get; set; } = "";    
        public string Language { get; set; } = "";
        public string Currency { get; set; } = "";
        public decimal Money { get; set; }      
        public string Deferred { get; set; } = "";    
        public string PercentType { get; set; } = "";
        public string Status { get; set; } = "";    
        public string? Wallet { get; set; }          // char(1) NULL
        public string? SeamlessUrl { get; set; }          // varchar(128) NULL
    }
    
}