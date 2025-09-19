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
                Url = gameModel.Url,
                Status = gameModel.Status,
                Top = gameModel.Top,
                Trending = gameModel.Trending,
                Datetime = gameModel.Datetime,
            };
            
            if (gameModel.Gamecode != null)
            {
                var nameMultiLang = JsonSerializer.Deserialize<Dictionary<string, string>>(
                    gameModel.Gamecode.NameMultiLanguage ?? "{}"
                ) ?? new();

                dto.Gamecode = new GamecodeDto
                {
                    Id = gameModel.Gamecode.Id,
                    Code = gameModel.Gamecode.Code,
                    Name = gameModel.Gamecode.Name,
                    NameMultiLanguage = nameMultiLang,
                };
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
                Url = Convert.ToString(row["game_url"]) ?? "",
                Status = Convert.ToChar(row["game_status"]),
                Top = Convert.ToChar(row["game_top"]),
                Trending = Convert.ToChar(row["game_trending"]),
                Datetime = Convert.ToDateTime(row["game_datetime"]),
            };

            if (row.Table.Columns.Contains("gamecode_id"))
            {
                game.Gamecode = new Gamecode
                {
                    Id = Convert.ToByte(row["gamecode_id"]),
                    Code = Convert.ToString(row["gamecode_code"]) ?? "",
                    Name = Convert.ToString(row["gamecode_name"]) ?? "",
                    NameMultiLanguage = Convert.ToString(row["gamecode_name_multi_language"]) ?? "",
                };
            }
            
            return game;
        }
    }
}