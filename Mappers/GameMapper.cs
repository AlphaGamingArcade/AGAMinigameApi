using System.Data;
using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Models;

namespace api.Mappers
{
    public static class GameMapper
    {
        public static GameDto ToGameDto(this Game gameModel)
        {
            return new GameDto
            {
                Id = gameModel.Id,
                Code = gameModel.Code,
                Name = gameModel.Name,
                Description = gameModel.Description,
                Image = gameModel.Image,
                Url = gameModel.Url,
                Status = gameModel.Status,
                Top = gameModel.Top,
                Trending = gameModel.Trending,
                Datetime =  gameModel.Datetime,
             };
        }

        public static Game ToGameFromDataRow(this DataRow reader)
        {
            return new Game
            {
                Id = Convert.ToInt32(reader["game_id"]),
                Code = Convert.ToString(reader["game_code"]) ?? "",
                Name = Convert.ToString(reader["game_name"]) ?? "",
                Description = Convert.ToString(reader["game_description"]) ?? "",
                Image = Convert.ToString(reader["game_image"]) ?? "",
                Url = Convert.ToString(reader["game_url"]) ?? "",
                Status = Convert.ToChar(reader["game_status"]),
                Top = Convert.ToChar(reader["game_top"]),
                Trending = Convert.ToChar(reader["game_trending"]),
                Datetime =  Convert.ToDateTime(reader["game_created_at"]),
            };
        }
    }
}