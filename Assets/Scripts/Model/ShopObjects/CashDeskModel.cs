using Data;
using UnityEngine;

namespace Model.ShopObjects
{
    public class CashDeskModel : ShopObjectModelBase
    {
        public CashDeskModel(Vector2Int cellCoords) 
            : base(cellCoords)
        {
            
        }

        public override ShopObjectType ShopObjectType => ShopObjectType.CashDesk;
    }
}