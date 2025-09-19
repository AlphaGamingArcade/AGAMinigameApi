using System.Data;
using System.Text.Json;
using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Dtos.Betting;
using AGAMinigameApi.Models;
using Namotion.Reflection;

namespace api.Mappers
{
    public static class BettingMapper
    {
        public static BettingDto ToBettingDto(this Betting bettingModel)
        {                
            var betting = new BettingDto
            {
                Id = bettingModel.Id,
                MemberId = bettingModel.MemberId,
                Money = bettingModel.Money,
                Benefit = bettingModel.Benefit,
                Result = bettingModel.Result,
                Datetime = bettingModel.Datetime
            };

            if (bettingModel.Gamecode != null)
            {
                var nameMultiLang = JsonSerializer.Deserialize<Dictionary<string, string>>(
                    bettingModel.Gamecode.NameMultiLanguage ?? "{}"
                ) ?? new();

                betting.Gamecode = new GamecodeDto
                {
                    Id = bettingModel.Gamecode.Id,
                    Code = bettingModel.Gamecode.Code,
                    Name = bettingModel.Gamecode.Name,
                    NameMultiLanguage = nameMultiLang,
                };
            }

            return betting;
        }


        public static Betting ToBettingFromDataRow(this DataRow row)
        {
            var betting = new Betting
            {
                Id = Convert.ToInt32(row["betting_id"]),
                MemberId = Convert.ToInt32(row["betting_member_id"]),
                GamecodeId = Convert.ToInt32(row["betting_gamecode_id"]),
                AgentId = Convert.ToInt16(row["betting_agent_id"]),
                Money = Convert.ToDecimal(row["betting_money"]),
                Benefit = Convert.ToDecimal(row["betting_benefit"]),
                Result = Convert.ToChar(row["betting_result"]),
                Datetime = Convert.ToDateTime(row["betting_datetime"]),
            };

            if (row.Table.Columns.Contains("gamecode_id"))
            {
                betting.Gamecode = row.ToGamecodeFromDataRow();
            }

            return betting;
        }
    }
}