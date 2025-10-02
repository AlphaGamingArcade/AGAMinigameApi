using System.Data;
using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Dtos.Play;
using AGAMinigameApi.Models;

namespace api.Mappers
{
    public static class PlayMapper
    {
        public static Play ToPlayFromDataRow(this DataRow row)
        {
            var play = new Play
            {
                Id = Convert.ToInt64(row["play_id"]),
                MemberId = Convert.ToInt32(row["play_member_id"]),
                GameId = Convert.ToInt32(row["play_game_id"]),
                CreatedAt = Convert.ToDateTime(row["play_created_at"]),
                UpdatedAt = Convert.ToDateTime(row["play_updated_at"]),
            };
            return play;
        }

        public static PlayDto ToPlayDto(this Play playModel)
        {
            var dto = new PlayDto
            {
                Id =  playModel.Id,
                MemberId = playModel.MemberId,
                GameId =  playModel.GameId,
                CreatedAt = playModel.CreatedAt,
                UpdatedAt = playModel.UpdatedAt
            };

            return dto;
        }
    }
}