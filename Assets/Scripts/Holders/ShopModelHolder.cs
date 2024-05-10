using Infra.Instance;
using Model;

namespace Holders
{
    public class ShopModelHolder : IShopModelHolder
    {
        public ShopModel ShopModel => Instance.Get<IPlayerModelHolder>().PlayerModel.ShopModel;
    }

    public interface IShopModelHolder
    {
        public ShopModel ShopModel { get; }
    }
}