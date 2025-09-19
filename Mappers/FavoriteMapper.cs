using System.Data;
using AGAMinigameApi.Dtos.Favorite;
using AGAMinigameApi.Models;

namespace api.Mappers
{
    public static class FavoriteMapper
    {
        public static FavoriteDto ToFavoriteDto(this Favorite favoriteModel)
        {
            return new FavoriteDto
            {
                Id = favoriteModel.Id,
                MemberId = favoriteModel.MemberId,
                GameId = favoriteModel.GameId,
                CreatedAt = favoriteModel.CreatedAt,
                UpdatedAt = favoriteModel.UpdatedAt
            };
        }


        public static Favorite ToFavoriteFromDataRow(this DataRow row)
        {
            var favorite = new Favorite
            {
                Id = Convert.ToInt32(row["favorite_id"]),
                MemberId = Convert.ToInt32(row["favorite_member_id"]),
                GameId = Convert.ToInt32(row["favorite_game_id"]),
                CreatedAt = Convert.ToDateTime(row["favorite_created_at"]),
                UpdatedAt = row["favorite_updated_at"] == DBNull.Value 
                    ? null 
                    : Convert.ToDateTime(row["favorite_updated_at"]) 
            };

            return favorite;
        }
    }
}