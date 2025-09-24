using System.ComponentModel.DataAnnotations;

namespace AGAMinigameApi.Dtos.Member
{
    public class UpdateMemberPasswordDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(64)]
        public string? Nickname { get; set; }
    }
}