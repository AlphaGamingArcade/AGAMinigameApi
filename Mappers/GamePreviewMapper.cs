using System.Data;
using AGAMinigameApi.Dtos.GamePreview;
using AGAMinigameApi.Models;

namespace api.Mappers
{
    public static class GamePreviewMapper
    {
        public static GamePreviewDto ToGamePreviewDto(this GamePreview model)
        {
            var dto = new GamePreviewDto
            {
                Id = model.Id,
                GameId = model.GameId,
                Image = model.Image,
                Title = model.Title,
                Description = model.Description,
                Datetime = model.Datetime,
            };

            return dto;
        }

        public static GamePreview ToGamePreviewFromDataRow(this DataRow row)
        {
            var dto = new GamePreview
            {
                Id = Convert.ToInt32(row["game_preview_id"]),
                GameId = Convert.ToInt16(row["game_preview_game_id"]),
                Image = Convert.ToString(row["game_preview_image"]) ?? "",
                Title = Convert.ToString(row["game_preview_title"]) ?? "",
                Description = Convert.ToString(row["game_preview_description"]) ?? "",
                Order = Convert.ToInt32(row["game_preview_order"]),
                Datetime = Convert.ToDateTime(row["game_preview_datetime"]),
            };
            return dto;
        }
    }
}