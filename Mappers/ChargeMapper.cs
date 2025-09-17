using System.Data;
using AGAMinigameApi.Dtos.Charge;
using AGAMinigameApi.Models;

namespace api.Mappers
{
    public static class ChargeMapper
    {
        public static ChargeDto ToChargeDto(this Charge chargeModel)
        {
            return new ChargeDto
            {
                Id = chargeModel.Id,
                MemberId = chargeModel.MemberId,
                Gamemoney = chargeModel.Gamemoney,
                Currency = chargeModel.Currency,
                Date = chargeModel.Date,
                Datetime =  chargeModel.Datetime,
             };
        }

        public static Charge ToChargeFromDataRow(this DataRow reader)
        {
            return new Charge
            {
                Id = Convert.ToInt64(reader["charge_id"]),
                MemberId = Convert.ToInt32(reader["charge_member_id"]),
                AgentId = Convert.ToInt16(reader["charge_agent_id"]),
                Gamemoney = Convert.ToDecimal(reader["charge_gamemoney"]),
                Currency = Convert.ToString(reader["charge_currency"]) ?? "",
                Date = DateOnly.FromDateTime(Convert.ToDateTime(reader["charge_datetime"])),
                Datetime =  Convert.ToDateTime(reader["charge_datetime"]),
            };
        }
    }
}