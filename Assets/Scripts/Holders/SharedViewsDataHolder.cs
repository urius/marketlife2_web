using System.Collections.Generic;
using Model.People;
using Model.ShopObjects;
using UnityEngine;
using View.Game.People;
using View.Game.ShopObjects.CashDesk;
using View.Game.ShopObjects.Shelf;
using View.Game.ShopObjects.TruckPoint;
using View.UI.BottomPanel;
using View.UI.SettingsCanvas;

namespace Holders
{
    public class SharedViewsDataHolder : ISharedViewsDataHolder
    {
        private readonly Dictionary<TruckPointModel, ITruckBoxPositionsProvider> _truckPointsDictionary = new();
        private readonly Dictionary<ShelfModel, IShelfProductSlotPositionProvider> _shelfSlotsDictionary = new();
        private readonly Dictionary<CashDeskModel, ICashDeskMoneyPositionProvider> _cashDeskProvidersDictionary = new();
        private readonly Dictionary<ProductBoxModel, ICharProductsInBoxPositionsProvider> _productsInBoxPositionsProviders = new();
        
        private IUICashDeskPanelTransformsProvider _uiCashDeskPanelTransformsProvider;
        private IUITruckPointPanelTransformsProvider _uiTruckPointPanelTransformsProvider;

        private IPlayerCharPositionsProvider _playerCharPositionsProvider;
        private RectTransform _topPanelInteriorButtonTransform;
        private RectTransform _topPanelDressesButtonTransform;
        private RectTransform _topPanelLeaderboardButtonTransform;
        private UISettingsCanvasView _settingsCanvasView;

        public void RegisterTruckBoxPositionProvider(TruckPointModel model, ITruckBoxPositionsProvider provider)
        {
            _truckPointsDictionary[model] = provider;
        }

        public void UnregisterTruckBoxPositionProvider(TruckPointModel model)
        {
            _truckPointsDictionary.Remove(model);
        }

        public ITruckBoxPositionsProvider GetTruckBoxPositionsProvider(TruckPointModel model)
        {
            return _truckPointsDictionary.TryGetValue(model, out var provider) ? provider : null;
        }

        public Vector3 GetTruckPointBoxPositions(TruckPointModel truckPointModel, int boxIndex)
        {
            return GetTruckBoxPositionsProvider(truckPointModel)
                .GetBoxWorldPosition(boxIndex);
        }

        public void RegisterPlayerCharPositionProvider(IPlayerCharPositionsProvider provider)
        {
            _playerCharPositionsProvider = provider;
        }

        public IPlayerCharPositionsProvider GetPlayerCharPositionProvider()
        {
            return _playerCharPositionsProvider;
        }

        public void UnregisterPlayerCharPositionProvider()
        {
            _playerCharPositionsProvider = null;
        }

        public void RegisterCharProductsInBoxPositionsProvider(ProductBoxModel productBoxModel, ICharProductsInBoxPositionsProvider provider)
        {
            _productsInBoxPositionsProviders[productBoxModel] = provider;
        }

        public ICharProductsInBoxPositionsProvider GetCharProductsInBoxPositionsProvider(ProductBoxModel productBoxModel)
        {
            return _productsInBoxPositionsProviders.TryGetValue(productBoxModel, out var provider) ? provider : null;
        }

        public void UnregisterCharProductsInBoxPositionsProvider(ProductBoxModel productBoxModel)
        {
            _productsInBoxPositionsProviders.Remove(productBoxModel);
        }

        public void RegisterShelfSlotPositionProvider(ShelfModel model, IShelfProductSlotPositionProvider provider)
        {
            _shelfSlotsDictionary[model] = provider;
        }

        public IShelfProductSlotPositionProvider GetShelfSlotPositionProvider(ShelfModel model)
        {
            return _shelfSlotsDictionary.TryGetValue(model, out var provider) ? provider : null;
        }

        public void UnregisterShelfSlotPositionProvider(ShelfModel model)
        {
            _shelfSlotsDictionary.Remove(model);
        }


        public void RegisterCashDeskMoneyPositionProvider(CashDeskModel model, ICashDeskMoneyPositionProvider provider)
        {
            _cashDeskProvidersDictionary[model] = provider;
        }

        public ICashDeskMoneyPositionProvider GetCashDeskMoneyPositionProvider(CashDeskModel model)
        {
            return _cashDeskProvidersDictionary.TryGetValue(model, out var provider) ? provider : null;
        }

        public void UnregisterCashDeskMoneyPositionProvider(CashDeskModel model)
        {
            _cashDeskProvidersDictionary.Remove(model);
        }

        public void RegisterCashDeskPanelTransformsProvider(IUICashDeskPanelTransformsProvider provider)
        {
            _uiCashDeskPanelTransformsProvider = provider;
        }

        public IUICashDeskPanelTransformsProvider GetCashDeskPanelTransformsProvider()
        {
            return _uiCashDeskPanelTransformsProvider;
        }

        public void UnregisterCashDeskPanelTransformsProvider()
        {
            _uiCashDeskPanelTransformsProvider = null;
        }
        
        public void RegisterTruckPointPanelTransformsProvider(IUITruckPointPanelTransformsProvider provider)
        {
            _uiTruckPointPanelTransformsProvider = provider;
        }

        public IUITruckPointPanelTransformsProvider GetTruckPointPanelTransformsProvider()
        {
            return _uiTruckPointPanelTransformsProvider;
        }

        public void UnregisterTruckPointPanelTransformsProvider()
        {
            _uiTruckPointPanelTransformsProvider = null;
        }
        
        

        public void RegisterTopPanelInteriorButtonTransform(RectTransform transform)
        {
            _topPanelInteriorButtonTransform = transform;
        }

        public RectTransform GetTopPanelInteriorButtonTransform()
        {
            return _topPanelInteriorButtonTransform;
        }

        public void UnregisterTopPanelInteriorButtonTransform()
        {
            _topPanelInteriorButtonTransform = null;
        }
        

        public void RegisterTopPanelDressesButtonTransform(RectTransform transform)
        {
            _topPanelDressesButtonTransform = transform;
        }

        public RectTransform GetTopPanelDressesButtonTransform()
        {
            return _topPanelDressesButtonTransform;
        }

        public void UnregisterTopPanelDressesButtonTransform()
        {
            _topPanelDressesButtonTransform = null;
        }
        

        public void RegisterTopPanelLeaderboardButtonTransform(RectTransform transform)
        {
            _topPanelLeaderboardButtonTransform = transform;
        }

        public RectTransform GetTopPanelLeaderboardButtonTransform()
        {
            return _topPanelLeaderboardButtonTransform;
        }

        public void UnregisterTopPanelLeaderboardButtonTransform()
        {
            _topPanelLeaderboardButtonTransform = null;
        }

        public void RegisterSettingsCanvasView(UISettingsCanvasView settingsCanvasView)
        {
            _settingsCanvasView = settingsCanvasView;
        }

        public UISettingsCanvasView GetSettingsCanvasView()
        {
            return _settingsCanvasView;
        }

        public void UnregisterSettingsCanvasView()
        {
            _settingsCanvasView = null;
        }
    }

    public interface ISharedViewsDataHolder
    {
        public void RegisterTruckBoxPositionProvider(TruckPointModel model, ITruckBoxPositionsProvider provider);
        public ITruckBoxPositionsProvider GetTruckBoxPositionsProvider(TruckPointModel model);
        public Vector3 GetTruckPointBoxPositions(TruckPointModel truckPointModel, int boxIndex);
        public void UnregisterTruckBoxPositionProvider(TruckPointModel model);
        
        public void RegisterPlayerCharPositionProvider(IPlayerCharPositionsProvider provider);
        public IPlayerCharPositionsProvider GetPlayerCharPositionProvider();
        public void UnregisterPlayerCharPositionProvider();
        
        public void RegisterCharProductsInBoxPositionsProvider(ProductBoxModel productBoxModel, ICharProductsInBoxPositionsProvider provider);
        public ICharProductsInBoxPositionsProvider GetCharProductsInBoxPositionsProvider(ProductBoxModel productBoxModel);
        public void UnregisterCharProductsInBoxPositionsProvider(ProductBoxModel productBoxModel);
        
        public void RegisterShelfSlotPositionProvider(ShelfModel model, IShelfProductSlotPositionProvider provider);
        public IShelfProductSlotPositionProvider GetShelfSlotPositionProvider(ShelfModel model);
        public void UnregisterShelfSlotPositionProvider(ShelfModel model);
        
        public void RegisterCashDeskMoneyPositionProvider(CashDeskModel model, ICashDeskMoneyPositionProvider provider);
        public ICashDeskMoneyPositionProvider GetCashDeskMoneyPositionProvider(CashDeskModel model);
        public void UnregisterCashDeskMoneyPositionProvider(CashDeskModel model);

        public void RegisterCashDeskPanelTransformsProvider(IUICashDeskPanelTransformsProvider provider);
        public IUICashDeskPanelTransformsProvider GetCashDeskPanelTransformsProvider();
        public void UnregisterCashDeskPanelTransformsProvider();
        
        public void RegisterTruckPointPanelTransformsProvider(IUITruckPointPanelTransformsProvider provider);
        public IUITruckPointPanelTransformsProvider GetTruckPointPanelTransformsProvider();
        public void UnregisterTruckPointPanelTransformsProvider();

        public void RegisterTopPanelInteriorButtonTransform(RectTransform transform);
        public RectTransform GetTopPanelInteriorButtonTransform();
        public void UnregisterTopPanelInteriorButtonTransform();

        public void RegisterTopPanelDressesButtonTransform(RectTransform transform);
        public RectTransform GetTopPanelDressesButtonTransform();
        public void UnregisterTopPanelDressesButtonTransform();

        public void RegisterTopPanelLeaderboardButtonTransform(RectTransform transform);
        public RectTransform GetTopPanelLeaderboardButtonTransform();
        public void UnregisterTopPanelLeaderboardButtonTransform();
        
        
        public void RegisterSettingsCanvasView(UISettingsCanvasView settingsCanvasView);
        public UISettingsCanvasView GetSettingsCanvasView();
        public void UnregisterSettingsCanvasView();
    }
}