using System.Data;
using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Models;
using Namotion.Reflection;

namespace api.Mappers
{
    public static class BettingMapper
    {
        public static BettingDto ToBettingDto(this Betting bettingModel)
        {
            return new BettingDto
            {
                Id = bettingModel.Id,
                MemberId = bettingModel.MemberId,
                GamecodeName = bettingModel.Gamecode?.Name ?? "",
                Money = bettingModel.Money,
                Benefit = bettingModel.Benefit,
                Result = bettingModel.Result,
                Datetime = bettingModel.Datetime
            };
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

            // Populate nested Gamecode only when the joined columns exist
            if (row.HasProperty("gc_id"))
            {
                betting.Gamecode = new Gamecode
                {
                    Id = Convert.ToByte(row["gc_id"]),
                    Code = Convert.ToString(row["gc_code"]) ?? string.Empty,
                    Name = Convert.ToString(row["gc_name"]) ?? string.Empty,
                    NameMultiLanguage = Convert.ToString(row["gc_name_multi_language"]) ?? string.Empty,
                    Percent = row.HasProperty("gc_percent") ? Convert.ToDouble(row["gc_percent"]) : 0d,
                    Datetime = row.HasProperty("gc_datetime") ? Convert.ToDateTime(row["gc_datetime"]) : default,
                    Status = Convert.ToString(row["gc_status"]) ?? string.Empty,
                    Order = row.HasProperty("gc_order") ? Convert.ToByte(row["gc_order"]) : (byte)0,
                    GameType = Convert.ToString(row["gc_game_type"]) ?? string.Empty
                };
            }

            return betting;
        }
    }
}