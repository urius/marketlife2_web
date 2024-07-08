using UnityEngine;

namespace View.Game.ShopObjects.CashDesk
{
    public interface ICashDeskMoneyPositionProvider
    {
        public Vector3 GetMoneySlotWorldPosition(int moneyPositionIndex);
    }
}