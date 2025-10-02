using System.Data;
using System.Text.Json;
using AGAMinigameApi.Dtos.Gamecode;
using AGAMinigameApi.Models;
using Namotion.Reflection;

namespace api.Mappers
{
    public static class GamecodeMapper
    {
        public static GamecodeDto ToGamecodeDto(this Gamecode gamecodeModel){
            var nameMultiLang = JsonSerializer.Deserialize<Dictionary<string, string>>(
                gamecodeModel.NameMultiLanguage ?? "{}"
            ) ?? new();

            return  new GamecodeDto
            {
                Id = gamecodeModel.Id,
                Code = gamecodeModel.Code,
                Name = gamecodeModel.Name,
                NameMultiLanguage = nameMultiLang,
                GameType = gamecodeModel.GameType
            };
        }

        public static Gamecode ToGamecodeFromDataRow(this DataRow row)
        {
            var game = new Gamecode
            {
                Id = Convert.ToByte(row["gamecode_id"]),
                Code = Convert.ToString(row["gamecode_code"]) ?? string.Empty,
                Name = Convert.ToString(row["gamecode_name"]) ?? string.Empty,
                NameMultiLanguage = Convert.ToString(row["gamecode_name_multi_language"]) ?? string.Empty,
                Percent = Convert.ToDouble(row["gamecode_percent"]),
                Datetime = Convert.ToDateTime(row["gamecode_datetime"]),
                Status = Convert.ToChar(row["gamecode_status"]),
                Order = Convert.ToByte(row["gamecode_order"]),
                GameType = Convert.ToChar(row["gamecode_game_type"])
            };

            return game;
        }
    }
}