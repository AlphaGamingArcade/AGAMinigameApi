using System.ComponentModel.DataAnnotations;

namespace SmptOptions
{
    public sealed class AppOptions
    {
        [Required]
        public string Key { get; set; } = "";

        [Required]
        public UrlsOptions Urls { get; set; } = new();

        public sealed class UrlsOptions
        {
            [Required, Url]
            public string PublicBaseUrl { get; set; } = "";
        }
    }
}