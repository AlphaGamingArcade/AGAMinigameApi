using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Dtos.Gamecode;

namespace AGAMinigameApi.Dtos.Betting
{
    public class BettingDto
    {
        public long Id { get; set; }
        public int MemberId { get; set; }
        public decimal Money { get; set; }
        public decimal Benefit { get; set; }
        public char Result { get; set; }
        public DateTime Datetime { get; set; }

        // extra
        public GamecodeDto? Gamecode { get; set; }
    }
}