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
                Name         = row.Field<string?>("agent_name")        ?? "",
                Language     = row.Field<string?>("agent_language")    ?? "",
                Currency     = row.Field<string?>("agent_currency")    ?? "",
                Money        = row.Field<decimal>("agent_money"),
                Deferred     = row.Field<string?>("agent_deferred")    ?? "",
                PercentType  = row.Field<string?>("agent_percent_type")?? "",
                Status       = row.Field<string?>("agent_status")      ?? "",
                Wallet       = row.Field<string?>("agent_wallet"),
                SeamlessUrl  = row.Field<string?>("agent_seamless_url")
            };
        }
    }
}