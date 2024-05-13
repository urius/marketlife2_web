using System;
using UnityEngine;
using View.Game.ShopObjects.Common;

namespace View.Game.ShopObjects.CashDesk
{
    [SelectionBase]
    public class CashDeskView : MonoBehaviour
    {
        public OwnedCellView[] OwnedCellViews { get; private set; }

        private void Awake()
        {
            OwnedCellViews = GetComponentsInChildren<OwnedCellView>();
            
            foreach (var ownedCellView in OwnedCellViews)
            {
                ownedCellView.gameObject.SetActive(false);
            }
        }
    }
}