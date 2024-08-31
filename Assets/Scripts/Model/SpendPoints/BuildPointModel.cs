using System;
using Data;
using UnityEngine;

namespace Model.SpendPoints
{
    public class BuildPointModel
    {
        public event Action MoneyToBuildLeftChanged;

        public readonly BuildPointType BuildPointType;
        public readonly ShopObjectType ShopObjectType;
        public readonly Vector2Int CellCoords;

        public BuildPointModel(BuildPointType buildPointType, ShopObjectType shopObjectType, Vector2Int cellCoords, int moneyToBuildLeft)
        {
            ShopObjectType = shopObjectType;
            BuildPointType = buildPointType;
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