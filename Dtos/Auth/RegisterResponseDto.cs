namespace AGAMinigameApi.Dtos.Auth
{
    public class RegisterResponseDto
    {
        public string? RefreshToken { get; set; }
        public string? AccessToken { get; set; }
    }
}