using System;
using Data;
using UnityEngine;

namespace Model.BuildPoint
{
    public class BuildPointModel
    {
        public event Action MoneyToBuildLeftChanged;
        
        public readonly ShopObjectType ShopObjectType;
        public readonly Vector2Int CellCoords;

        public BuildPointModel(ShopObjectType shopObjectType, Vector2Int cellCoords, int moneyToBuildLeft)
        {
            ShopObjectType = shopObjectType;
            CellCoords = cellCoords;
            MoneyToBuildLeft = moneyToBuildLeft;
        }

        public int MoneyToBuildLeft { get; private set; }

        public void ChangeMoneyToBuildLeft(int deltaMoney)
        {
            MoneyToBuildLeft += deltaMoney;
            
            MoneyToBuildLeftChanged?.Invoke();
        }
    }
}