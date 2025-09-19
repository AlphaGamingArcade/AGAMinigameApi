using System.ComponentModel.DataAnnotations;

namespace AGAMinigameApi.Dtos.Favorite
{
    public class CreateFavoriteDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "GameId must be a positive integer.")]
        public int GameId { get; set; }
    }
}
