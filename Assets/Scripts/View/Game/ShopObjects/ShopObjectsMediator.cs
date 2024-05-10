using System;
using Data;
using Holders;
using Infra.Instance;
using Model.ShopObjects;
using View.Game.ShopObjects.CashDesk;

namespace View.Game.ShopObjects
{
    public class ShopObjectsMediator : MediatorBase
    {
        private readonly IShopModelHolder _shopModelHolder = Instance.Get<IShopModelHolder>();

        protected override void MediateInternal()
        {
            foreach (var kvp in _shopModelHolder.ShopModel.ShopObjects)
            {
                MediateShopObject(kvp.Value);
            }
        }

        private void MediateShopObject(ShopObjectModelBase shopObjectModel)
        {
            switch (shopObjectModel.ShopObjectType)
            {
                case ShopObjectType.CashDesk:
                    MediateChild<CashDeskMediator, CashDeskModel>(TargetTransform, (CashDeskModel)shopObjectModel);
                    break;
                default:
                    throw new NotSupportedException(
                        $"Not supported mediator for shop object type: ${shopObjectModel.ShopObjectType}");
            }
        }

        protected override void UnmediateInternal()
        {
            throw new System.NotImplementedException();
        }
    }
}