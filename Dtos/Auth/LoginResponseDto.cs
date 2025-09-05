namespace AGAMinigameApi.Dtos.Auth
{
    public class LoginResponseDto
    {
        public int Sub { get; set; }
        public string? RefreshToken { get; set; }
        public string? AccessToken { get; set; }
    }
}