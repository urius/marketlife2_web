using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Dto;
using Data.Dto.ShopObjects;
using Holders;
using Infra.Instance;
using Model;
using Model.People;
using Model.ShopObjects;
using Model.SpendPoints;

namespace Utils
{
    public static class DataConverter
    {
        private static readonly IShelfUpgradeSettingsProvider _shelfSettingsProvider = Instance.Get<IShelfUpgradeSettingsProvider>();
        private static readonly TruckPointsSettingsProviderSo _truckPointsSettingsProvider = Instance.Get<TruckPointsSettingsProviderSo>();
        
        public static PlayerModel ToPlayerModel(this PlayerDataDto dataDto)
        {
            var shopModel = ToShopModel(dataDto.ShopData);
            var playerCharData = ToPlayerCharModel(dataDto.PlayerCharData);
            var audioSettingsModel = ToAudioSettingsModel(dataDto.AudioSettings);
            var uiFLagsModel = ToUiFLagsModel(dataDto.UIFlags);
            var statsModel = ToStatsModel(dataDto.Stats);

            var result = new PlayerModel(
                shopModel,
                dataDto.Money,
                dataDto.Level,
                dataDto.StaffWorkTimeSeconds,
                dataDto.BoughtWalls,
                dataDto.BoughtFloors,
                playerCharData,
                dataDto.PassedTutorialSteps,
                audioSettingsModel,
                uiFLagsModel,
                statsModel);

            return result;
        }

        public static PlayerDataDto ToPlayerDataDto(this PlayerModel playerModel)
        {
            var shopDataDto = ToShopDataDto(playerModel.ShopModel);
            var playerCharDto = ToPlayerCharDto(playerModel.PlayerCharModel);
            var audioSettingsDto = ToAudioSettingsDto(playerModel.AudioSettingsModel);
            var iuFLagsDto = ToUiFlagsDto(playerModel.UIFlagsModel);
            var statsDto = ToPlayerStatsDto(playerModel.StatsModel);

            return new PlayerDataDto(
                shopDataDto,
                playerModel.MoneyAmount,
                playerModel.Level,
                playerModel.StaffWorkTimeSeconds,
                playerModel.BoughtWalls.ToArray(),
                playerModel.BoughtFloors.ToArray(),
                playerCharDto,
                playerModel.PassedTutorialSteps.ToArray(),
                audioSettingsDto,
                iuFLagsDto,
                statsDto);
        }

        public static BuildPointModel ToBuildPointModel(BuildPointDto buildPointDto)
        {
            return new BuildPointModel(
                buildPointDto.BuildPointType, 
                buildPointDto.ShopObjectType,
                buildPointDto.CellCoords,
                buildPointDto.MoneyToBuildLeft);
        }

        private static PlayerStatsDto ToPlayerStatsDto(PlayerStatsModel model)
        {
            return new PlayerStatsDto(model.TotalMoneyEarned);
        }

        private static PlayerUIFlagsDto ToUiFlagsDto(PlayerUIFlagsModel model)
        {
            return new PlayerUIFlagsDto(model.HaveNewWalls, model.HaveNewFloors);
        }

        private static AudioSettingsDto ToAudioSettingsDto(PlayerAudioSettingsModel model)
        {
            return new AudioSettingsDto(model.IsSoundsMuted, model.IsMusicMuted);
        }

        private static PlayerCharDataDto ToPlayerCharDto(PlayerCharModel model)
        {
            return new PlayerCharDataDto(model.CellPosition, ToProductsDto(model.ProductsInBox));
        }

        private static ShopDataDto ToShopDataDto(ShopModel shopModel)
        {
            var cashDesks = shopModel.CashDesks.Select(ToCashDeskDto).ToArray();
            var shelfs = shopModel.Shelfs.Select(ToShelfDto).ToArray();
            var truckPoints = shopModel.TruckPoints.Select(ToTruckPointDto).ToArray();
            var buildPoints = shopModel.BuildPoints.Values.Select(ToBuildPointDto).ToArray();

            return new ShopDataDto(
                shopModel.Size,
                shopModel.WallsType,
                shopModel.FloorsType,
                cashDesks,
                shelfs,
                truckPoints,
                buildPoints);
        }

        private static BuildPointDto ToBuildPointDto(BuildPointModel model)
        {
            return new BuildPointDto(
                model.BuildPointType,
                model.ShopObjectType,
                model.CellCoords,
                model.MoneyToBuildLeft);
        }

        private static TruckPointDto ToTruckPointDto(TruckPointModel model)
        {
            var productBoxes = ToProductsDto(model.CurrentProductBoxes);
            var truckPontStaff = ToTruckPointStaffDto(model.StaffCharModel);
            
            return new TruckPointDto(
                model.CellCoords,
                productBoxes,
                model.UpgradesCount,
                model.DeliverTimeSecondsRest,
                truckPontStaff);
        }

        private static TruckPointStaffCharDto ToTruckPointStaffDto(TruckPointStaffCharModel model)
        {
            if (model == null) return default;
            
            return new TruckPointStaffCharDto(
                model.CellCoords,
                model.WorkSecondsLeft,
                ToProductsDto(model.ProductsBox.ProductsInBox));
        }

        private static CashDeskDto ToCashDeskDto(CashDeskModel model)
        {
            var staffWorkTime = model.HasCashMan ? model.CashDeskStaffModel.WorkSecondsLeft : 0;
            
            return new CashDeskDto(model.CellCoords, staffWorkTime, model.MoneyAmount);
        }

        private static ShelfDto ToShelfDto(ShelfModel model)
        {
            return new ShelfDto(
                model.ShopObjectType,
                model.CellCoords,
                model.UpgradeIndex,
                ToProductsDto(model.ProductSlots));
        }

        private static PlayerStatsModel ToStatsModel(PlayerStatsDto dto)
        {
            return new PlayerStatsModel(dto.TotalMoneyEarned);
        }

        private static PlayerUIFlagsModel ToUiFLagsModel(PlayerUIFlagsDto dto)
        {
            return new PlayerUIFlagsModel(dto.HaveNewWalls, dto.HaveNewFloors);
        }

        private static PlayerAudioSettingsModel ToAudioSettingsModel(AudioSettingsDto dto)
        {
            return new PlayerAudioSettingsModel(dto.IsSoundsMuted, dto.IsMusicMuted);
        }

        private static PlayerCharModel ToPlayerCharModel(PlayerCharDataDto playerCharDataDto)
        {
            return new PlayerCharModel(playerCharDataDto.CellPosition, ToProductTypes(playerCharDataDto.ProductsInBox));
        }

        private static ShopModel ToShopModel(ShopDataDto shopDataDto)
        {
            var shopObjects = ToShopObjects(shopDataDto.CashDesks, shopDataDto.Shelfs, shopDataDto.TruckPoints);
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

        private static IEnumerable<ShopObjectModelBase> ToShopObjects(CashDeskDto[] cashDesks, ShelfDto[] shelfs, TruckPointDto[] truckPoints)
        {
            var result = new List<ShopObjectModelBase>(cashDesks.Length + shelfs.Length + truckPoints.Length);

            foreach (var cashDeskDto in cashDesks)
            {
                result.Add(ToCashDeskModel(cashDeskDto));
            }

            foreach (var shelf in shelfs)
            {
                var shelfModel = ToShelfModel(shelf);
                if (shelfModel != null)
                {
                    result.Add(shelfModel);
                }
            }

            for (var i = 0; i < truckPoints.Length; i++)
            {
                var truckPointDto = truckPoints[i];
                var truckPointModel = ToTruckPointModel(truckPointDto, i);
                if (truckPointModel != null)
                {
                    result.Add(truckPointModel);
                }
            }

            return result;
        }

        private static TruckPointModel ToTruckPointModel(TruckPointDto dto, int truckPointIndex)
        {
            if (_truckPointsSettingsProvider.TryGetSettingByTruckPointIndex(truckPointIndex,
                    out var truckPointSetting))
            {
                return new TruckPointModel(dto.CellCoords,
                    truckPointSetting,
                    ToProductTypes(dto.ProductBoxes),
                    dto.UpgradesCount,
                    dto.DeliverTimeSecondsRest,
                    ToTruckPointStaffModel(dto.StaffData));
            }

            return null;
        }

        private static TruckPointStaffCharModel ToTruckPointStaffModel(TruckPointStaffCharDto dto)
        {
            if (dto.WorkSecondsLeft > 0)
            {
                return new TruckPointStaffCharModel(dto.CellCoords, dto.WorkSecondsLeft, ToProductTypes(dto.Products));
            }

            return null;
        }

        private static ShopObjectModelBase ToShelfModel(ShelfDto dto)
        {
            if (_shelfSettingsProvider.TryGetShelfUpgradeSetting(dto.ShelfType, dto.UpgradeIndex,
                    out var shelfSettings))
            {
                return new ShelfModel(
                    dto.CellCoords,
                    dto.ShelfType,
                    shelfSettings.SlotsAmount,
                    dto.UpgradeIndex,
                    ToProductTypes(dto.Products));
            }

            return null;
        }

        private static ProductType[] ToProductTypes(int[] intProducts)
        {
            var productTypes = new ProductType[intProducts.Length];
    
            for (var i = 0; i < intProducts.Length; i++)
            {
                productTypes[i] = (ProductType)intProducts[i];
            }
    
            return productTypes;
        }
        
        private static int[] ToProductsDto(IReadOnlyList<ProductType> productTypes)
        {
            var result = new int[productTypes.Count];
    
            for (var i = 0; i < productTypes.Count; i++)
            {
                result[i] = (int)productTypes[i];
            }
    
            return result;
        }

        private static CashDeskModel ToCashDeskModel(CashDeskDto dto)
        {
            var staff = dto.StaffWorkTimeSecond > 0
                ? new CashDeskStaffModel(
                    dto.CellCoords + Constants.CashDeskStaffPositionOffset, dto.StaffWorkTimeSecond)
                : null;
            
            return new CashDeskModel(dto.CellCoords, staff);
        }
    }
}