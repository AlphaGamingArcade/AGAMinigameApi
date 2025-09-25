using System.Data;
using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Models;
using Microsoft.Data.SqlClient;

namespace api.Mappers
{
    public static class CommentMapper
    {
        public static BannerDto ToBannerDto(this Banner bannerModel)
        {
            return new BannerDto
            {
                Id = bannerModel.Id,
                Title = bannerModel.Title,
                Description = bannerModel.Description,
                Image = bannerModel.Image,
                Url = bannerModel.Url,
                Order = bannerModel.Order,
                Datetime = bannerModel.Datetime
            };
        }

        public static Banner ToBannerFromCreate(this CreateBannerDto bannerDto, int bannerId)
        {
            return new Banner
            {
                
            };
        }

        public static Banner ToBannerFromUpdate(this UpdateBannerDto commentDto, int bannerId)
        {
            return new Banner
            {

            };
        }
        
        public static Banner ToBannerFromDataRow(this DataRow reader)
        {
            return new Banner
            {
                Id = Convert.ToInt32(reader["banner_id"]),
                Title = Convert.ToString(reader["banner_title"]),
                Description = Convert.ToString(reader["banner_description"]),
                Image = Convert.ToString(reader["banner_image"]),
                Url = Convert.ToString(reader["banner_url"]),
                Order = Convert.ToInt32(reader["banner_order"]),
                Datetime = Convert.ToDateTime(reader["banner_datetime"]),
            };
        }
    }
}