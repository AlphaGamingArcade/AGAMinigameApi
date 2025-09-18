using System.Data;
using AGAMinigameApi.Dtos.Betting;
using AGAMinigameApi.Dtos.Favorite;
using AGAMinigameApi.Models;
using Namotion.Reflection;

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
                ItemType = favoriteModel.ItemType,
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
                ItemType = Convert.ToString(row["favorite_item_type"]) ?? "",
                ItemId = Convert.ToInt32(row["favorite_item_id"]),
                CreatedAt = Convert.ToDateTime(row["favorite_created_at"]),
                UpdatedAt = Convert.ToDateTime(row["favorite_updated_at"])
            };

            return favorite;
        }
    }
}