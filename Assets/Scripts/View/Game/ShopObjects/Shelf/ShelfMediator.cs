using System.IO;
using Holders;
using Infra.Instance;
using Model.ShopObjects;
using Utils;
using View.Game.Extensions;
using View.Helpers;

namespace View.Game.ShopObjects.Shelf
{
    public class ShelfMediator : ShopObjectMediatorBase<ShelfModel>
    {
        private readonly IShelfSettingsProvider _shelfSettingsProvider = Instance.Get<IShelfSettingsProvider>();
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly IOwnedCellsDataHolder _ownedCellsDataHolder = Instance.Get<IOwnedCellsDataHolder>();
        
        private ShelfView _view;
        
        protected override void MediateInternal()
        {
            if (_shelfSettingsProvider.TryGetShelfSetting(TargetModel.ShopObjectType, TargetModel.UpgradeIndex, out var shelfSettings))
            {
                _view = InstantiatePrefab<ShelfView>(shelfSettings.PrefabKey);
                
                _view.transform.position = _gridCalculator.GetCellCenterWorld(TargetModel.CellCoords);

                OwnCells();

                DisplayProducts();
            }
            else
            {
                throw new InvalidDataException(
                    $"Unable to get shelf setting for shelf type: {TargetModel.ShopObjectType}");
            }
        }

        protected override void UnmediateInternal()
        {
            _ownedCellsDataHolder.UnregisterShopObject(TargetModel);
            
            Destroy(_view);
            _view = null;
        }

        private void OwnCells()
        {
            var ownedCells = _gridCalculator.GetOwnedCells(_view);

            _ownedCellsDataHolder.RegisterShopObject(TargetModel, ownedCells);
        }

        private void DisplayProducts()
        {
            
        }

        protected override void UpdateSorting()
        {
            _view.SetSortingOrder(SortingOrderHelper.GetDefaultSortingOrderByCoords(TargetModel.CellCoords));
        }
    }
}