using System.Data;
using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Models;

namespace api.Mappers
{
    public static class RechargeMapper
    {
        public static RechargeDto ToRechargeDto(this Recharge rechargeModel)
        {
            return new RechargeDto
            {
                Id = rechargeModel.Id,
                MemberId = rechargeModel.MemberId,
                Gamemoney = rechargeModel.Gamemoney,
                Currency = rechargeModel.Currency,
                Date = rechargeModel.Date,
                Datetime =  rechargeModel.Datetime,
             };
        }

        public static Recharge ToRechargeFromDataRow(this DataRow reader)
        {
            return new Recharge
            {
                Id = Convert.ToInt64(reader["recharge_id"]),
                MemberId = Convert.ToInt32(reader["recharge_member_id"]),
                AgentId = Convert.ToInt16(reader["recharge_agent_id"]),
                Gamemoney = Convert.ToDecimal(reader["recharge_gamemoney"]),
                Currency = Convert.ToString(reader["recharge_currency"]) ?? "",
                Date = DateOnly.FromDateTime(Convert.ToDateTime(reader["recharge_datetime"])),
                Datetime =  Convert.ToDateTime(reader["recharge_datetime"]),
            };
        }
    }
}