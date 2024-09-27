using System;
using Data;
using Events;
using Extensions;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.ShopObjects;
using Tools.AudioManager;
using View.Game.ShopObjects.CashDesk;
using View.Game.ShopObjects.Shelf;
using View.Game.ShopObjects.TruckPoint;

namespace View.Game.ShopObjects
{
    public class ShopObjectsMediator : MediatorBase
    {
        private readonly IShopModelHolder _shopModelHolder = Instance.Get<IShopModelHolder>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        
        private ShopModel _shopModel;

        protected override void MediateInternal()
        {
            _shopModel = _shopModelHolder.ShopModel;
            
            foreach (var kvp in _shopModel.ShopObjects)
            {
                MediateShopObject(kvp.Value);
            }

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _shopModel.ShopObjectAdded += OnShopObjectAdded;
        }

        private void Unsubscribe()
        {
            _shopModel.ShopObjectAdded -= OnShopObjectAdded;
        }

        private void OnShopObjectAdded(ShopObjectModelBase model)
        {
            var useBigSmoke = model.ShopObjectType is ShopObjectType.CashDesk or ShopObjectType.TruckPoint;
            _eventBus.Dispatch(new VFXRequestSmokeEvent(model.CellCoords, useBigSmoke));

            _audioPlayer.PlaySound(SoundIdKey.ObjectPlacement);
            _audioPlayer.PlaySound(SoundIdKey.Puff);
            
            MediateShopObject(model);
        }

        private void MediateShopObject(ShopObjectModelBase shopObjectModel)
        {
            switch (shopObjectModel.ShopObjectType)
            {
                case ShopObjectType.CashDesk:
                    MediateChild<CashDeskMediator, CashDeskModel>(TargetTransform, (CashDeskModel)shopObjectModel);
                    break;
                case ShopObjectType.TruckPoint:
                    var truckPointModel = (TruckPointModel)shopObjectModel;
                    MediateChild<TruckPointMediator, TruckPointModel>(TargetTransform, truckPointModel);
                    break;
                default:
                    if (shopObjectModel.ShopObjectType.IsShelf())
                    {
                        MediateChild<ShelfMediator, ShelfModel>(TargetTransform, (ShelfModel)shopObjectModel);
                    }
                    else
                    {
                        throw new NotSupportedException(
                            $"Not supported mediator for shop object type: ${shopObjectModel.ShopObjectType}");
                    }
                    break;
            }
        }
    }
}