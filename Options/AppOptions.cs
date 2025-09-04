using System.ComponentModel.DataAnnotations;

namespace SmptOptions
{
    public sealed class AppOptions
    {
        [Required]
        public string Key { get; set; } = "";

        [Required]
        public string Url { get; set; } = "";
    }
}