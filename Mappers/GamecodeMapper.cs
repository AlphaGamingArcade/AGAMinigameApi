using System.Data;
using System.Text.Json;
using AGAMinigameApi.Dtos.Banner;
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
                Percent = row.HasProperty("gamecode_percent") ? Convert.ToDouble(row["gamecode_percent"]) : 0d,
                Datetime = row.HasProperty("gamecode_datetime") ? Convert.ToDateTime(row["gamecode_datetime"]) : default,
                Status = row.HasProperty("gamecode_status") ? Convert.ToChar(row["gamecode_status"]) : default,
                Order = row.HasProperty("gamecode_order") ? Convert.ToByte(row["gamecode_order"]) : (byte)0,
                GameType = Convert.ToString(row["gamecode_game_type"]) ?? string.Empty
            };
            return game;
        }
    }
}