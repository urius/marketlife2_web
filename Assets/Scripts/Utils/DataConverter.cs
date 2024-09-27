using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Dto;
using Data.Dto.ShopObjects;
using Model;
using Model.ShopObjects;
using Model.SpendPoints;

namespace Utils
{
    public static class DataConverter
    {
        public static PlayerModel ToPlayerModel(this PlayerDataDto dataDto)
        {
            var shopModel = ToShopModel(dataDto.ShopData);
            var playerCharData = ToPlayerCharModel(dataDto.PlayerCharData);
            var audioSettingsModel = ToAudioSettingsModel(dataDto.AudioSettings);

            var result = new PlayerModel(
                shopModel,
                dataDto.Money,
                dataDto.Level,
                dataDto.StaffWorkTimeSeconds,
                dataDto.UnlockedWalls,
                dataDto.UnlockedFloors,
                playerCharData,
                Enumerable.Empty<TutorialStep>(),
                audioSettingsModel);

            return result;
        }

        private static PlayerAudioSettingsModel ToAudioSettingsModel(AudioSettingsDto dto)
        {
            return new PlayerAudioSettingsModel(dto.IsAudioMuted, dto.IsMusicMuted);
        }

        public static BuildPointModel ToBuildPointModel(BuildPointDto buildPointDto)
        {
            return new BuildPointModel(
                buildPointDto.BuildPointType, 
                buildPointDto.ShopObjectType,
                buildPointDto.CellCoords,
                buildPointDto.MoneyToBuildLeft);
        }

        private static PlayerCharModel ToPlayerCharModel(PlayerCharDataDto playerCharDataDto)
        {
            return new PlayerCharModel(playerCharDataDto.CellPosition);
        }

        private static ShopModel ToShopModel(ShopDataDto shopDataDto)
        {
            var shopObjects = ToShopObjects(shopDataDto.ShopObjects);
            var buildPoints = ToBuildPoints(shopDataDto.BuildPoints);
            
            var result = new ShopModel(shopDataDto.Size, shopDataDto.WallType, shopDataDto.FloorType, shopObjects, buildPoints);

            return result;
        }

        private static IEnumerable<BuildPointModel> ToBuildPoints(IEnumerable<BuildPointDto> buildPoints)
        {
            return buildPoints
                .Select(ToBuildPointModel)
                .ToArray();
        }

        private static IEnumerable<ShopObjectModelBase> ToShopObjects(ShopObjectDto[] shopObjectDtos)
        {
            var result = new ShopObjectModelBase[shopObjectDtos.Length];
            for (var i = 0; i < shopObjectDtos.Length; i++)
            {
                var shopObjectDto = shopObjectDtos[i];
                result[i] = ToShopObject(shopObjectDto);
            }

            return result;
        }

        private static ShopObjectModelBase ToShopObject(ShopObjectDto shopObjectDto)
        {
            ShopObjectModelBase result;
            switch (shopObjectDto.ShopObjectType)
            {
                case ShopObjectType.CashDesk:
                    result = new CashDeskModel(shopObjectDto.CellCoords); //convert staff model
                    break;
                default:
                    throw new NotSupportedException(
                        $"Shop object type ${shopObjectDto.ShopObjectType} is not supported to convert");
            }

            return result;
        }
    }
}