using System.ComponentModel.DataAnnotations;

namespace AGAMinigameApi.Dtos.Favorite
{
    public class CreateFavoriteDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "GameId must be a positive integer.")]
        public int GameId { get; set; }

        // must be exactly one character
        [Required]
        [StringLength(1, MinimumLength = 1, ErrorMessage = "GameType must be a single character.")]
        public string GameType { get; set; } = string.Empty;
    }
}
