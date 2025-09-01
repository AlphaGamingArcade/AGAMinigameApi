using System.Data;
using AGAMinigameApi.Models;

namespace api.Mappers
{
    public static class AgentMapper
    {
        public static Agent ToAgentFromDataRow(this DataRow row)
        {
            return new Agent
            {
                Id           = row.Field<short>("agent_id"),
                Code         = row.Field<string?>("agent_code")        ?? "",
                Password     = row.Field<string?>("agent_password")    ?? "",
                Name         = row.Field<string?>("agent_name")        ?? "",
                UplevelId    = row.Field<short?>("agent_uplevel_id"),
                Language     = row.Field<string?>("agent_language")    ?? "",
                Currency     = row.Field<string?>("agent_currency")    ?? "",
                Money        = row.Field<decimal>("agent_money"),
                Datetime     = row.Field<DateTime>("agent_datetime"),
                Update       = row.Field<DateTime?>("agent_update"),
                Ip           = row.Field<string?>("agent_ip")          ?? "",
                Affiliate    = row.Field<string?>("agent_affiliate")   ?? "",
                Deferred     = row.Field<string?>("agent_deferred")    ?? "",
                PercentType  = row.Field<string?>("agent_percent_type")?? "",
                Status       = row.Field<string?>("agent_status")      ?? "",
                Multilogin   = row.Field<string?>("agent_multilogin")  ?? "",
                Session      = row.Field<string?>("agent_session"),
                Wallet       = row.Field<string?>("agent_wallet"),
                SeamlessUrl  = row.Field<string?>("agent_seamless_url"),
                ReturnUrl    = row.Field<string?>("agent_return_url")
            };
        }
    }
}