using Data;
using Model;

namespace Utils
{
    public static class DataConverter
    {
        public static PlayerModel ToPlayerModel(this PlayerDataDto dataDto)
        {
            var shopModel = ToShopModel(dataDto.ShopData);

            var result = new PlayerModel(shopModel, dataDto.Money);

            return result;
        }

        private static ShopModel ToShopModel(ShopDataDto shopDataDto)
        {
            var result = new ShopModel(shopDataDto.Size, shopDataDto.WallType);

            return result;
        }
    }
}