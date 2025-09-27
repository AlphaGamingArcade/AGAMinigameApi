using System.Data;
using AGAMinigameApi.Models;

namespace api.Mappers
{
    public static class PlayMapper
    {
        public static Play ToPlayFromDataRow(this DataRow row)
        {
            var play = new Play
            {
                MemberId = Convert.ToInt32(row["app_play_member_id"]),
                Gamecode = Convert.ToString(row["app_play_game_code"]) ?? "",
                CreatedAt = Convert.ToDateTime(row["app_play_created_at"]),
                UpdatedAt = row["app_play_updated_at"] == DBNull.Value ? null : Convert.ToDateTime(row["app_play_updated_at"]),
            };

            if (row.Table.Columns.Contains("game_code"))
            {
                play.Game = row.ToGameFromDataRow();
            }

            return play;
        }
    }
}