namespace AGAMinigameApi.Dtos.Gamecode
{
    public class GamecodeDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public object? NameMultiLanguage { get; set; }
        public char? GameType { get; set; }
    }
}