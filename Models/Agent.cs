namespace AGAMinigameApi.Models
{
    public class Agent
    {
        public short Id { get; set; } 
        public string Code { get; set; } = "";    // varchar(16)
        public string Password { get; set; } = "";    // varchar(32)
        public string Name { get; set; } = "";    // varchar(32)

        public short? UplevelId { get; set; }          // smallint NULL
        public string Language { get; set; } = "";

        public string Currency { get; set; } = "";    // char(3) NOT NULL
        public decimal Money { get; set; }          // decimal(19,2) NOT NULL

        public DateTime Datetime { get; set; }          // datetime NOT NULL
        public DateTime? Update { get; set; }          // datetime NULL

        public string Ip { get; set; } = "";    // varchar(39) NOT NULL
        public string Affiliate { get; set; } = "";    // char(1) NOT NULL
        public string Deferred { get; set; } = "";    // char(1) NOT NULL
        public string PercentType { get; set; } = "";    // char(1) NOT NULL
        public string Status { get; set; } = "";    // char(1) NOT NULL
        public string Multilogin { get; set; } = "";    // char(1) NOT NULL

        public string? Session { get; set; }          // varchar(32) NULL
        public string? Wallet { get; set; }          // char(1) NULL
        public string? SeamlessUrl { get; set; }          // varchar(128) NULL
        public string? ReturnUrl { get; set; }          // varchar(64) NULL
    }
    
}