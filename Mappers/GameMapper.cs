using System.Data;
using System.Text.Json;
using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Models;

namespace api.Mappers
{
    public static class GameMapper
    {
        public static GameDto ToGameDto(this Game gameModel)
        {
            var descMultiLang = JsonSerializer.Deserialize<Dictionary<string, string>>(
                gameModel.DescriptionMultiLanguage ?? "{}"
            ) ?? new();
                
            var dto = new GameDto
            {
                Code = gameModel.Code,
                Description = gameModel.Description,
                DescriptionMultiLanguage = descMultiLang,
                Image = gameModel.Image,
                PlayUrl = gameModel.PlayUrl,
                Status = gameModel.Status,
                Top = gameModel.Top,
                Trending = gameModel.Trending,
                Datetime = gameModel.Datetime,
            };
            
            if (gameModel.Gamecode != null)
            {
                dto.Gamecode = gameModel.Gamecode.ToGamecodeDto();
            }

            return dto;
        }

        public static Game ToGameFromDataRow(this DataRow row)
        {
            var game = new Game
            {
                Code = Convert.ToString(row["game_code"]) ?? "",
                Description = Convert.ToString(row["game_description"]) ?? "",
                DescriptionMultiLanguage = Convert.ToString(row["game_description_multi_language"]) ?? "{}",
                Image = Convert.ToString(row["game_image"]) ?? "",
                PlayUrl = Convert.ToString(row["game_play_url"]) ?? "",
                Status = Convert.ToChar(row["game_status"]),
                Top = Convert.ToChar(row["game_top"]),
                Trending = Convert.ToChar(row["game_trending"]),
                Datetime = Convert.ToDateTime(row["game_datetime"]),
            };

            if (row.Table.Columns.Contains("gamecode_id"))
            {
                game.Gamecode = row.ToGamecodeFromDataRow();
            }
            
            return game;
        }
    }
}