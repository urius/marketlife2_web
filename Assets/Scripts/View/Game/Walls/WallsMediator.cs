using System.Collections.Generic;
using Data;
using Holders;
using Infra.Instance;
using Model;
using Model.ShopObjects;
using UnityEngine;
using Utils;

namespace View.Game.Walls
{
    public class WallsMediator : MediatorBase
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        private readonly SpritesHolderSo _spritesHolder = Instance.Get<SpritesHolderSo>();
        private readonly Dictionary<Vector2Int, WallView> _wallByCoords = new();

        private ShopModel ShopModel => _playerModelHolder.PlayerModel.ShopModel;

        protected override void MediateInternal()
        {
            DisplayWalls();

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            RemoveWalls();
            
            Unsubscribe();
        }

        private void Subscribe()
        {
            ShopModel.ShopObjectAdded += OnShopObjectAdded;
        }

        private void Unsubscribe()
        {
            ShopModel.ShopObjectAdded -= OnShopObjectAdded;
        }

        private void OnShopObjectAdded(ShopObjectModelBase shopObjectModel)
        {
            if (shopObjectModel.ShopObjectType == ShopObjectType.TruckPoint)
            {
                RemoveWall(shopObjectModel.CellCoords.y);
                RemoveWall(shopObjectModel.CellCoords.y-1);
            }
        }

        private void RemoveWall(int yCoords)
        {
            var coords = new Vector2Int(-1, yCoords);
            
            if (_wallByCoords.TryGetValue(coords, out var wallView))
            {
                _wallByCoords.Remove(coords);
                Destroy(wallView);
            }
        }

        private void DisplayWalls()
        {
            RemoveWalls();
            
            var shopSize = ShopModel.Size;
            
            for (var x = 0; x < shopSize.x; x++)
            {
                var coords = new Vector2Int(x, -1);
                var wallView = CreateOrGetWall(coords);
                wallView.ToXMode(x == shopSize.x - 1);
            }
            
            for (var y = 0; y < shopSize.y; y++)
            {
                if (HaveTruckGatesOn(y)) continue;
                
                var coords = new Vector2Int(-1, y);
                var wallView = CreateOrGetWall(coords);
                wallView.ToYMode(y == shopSize.y - 1);
            }

            SetWallsSprite();
        }

        private void RemoveWalls()
        {
            foreach (var kvp in _wallByCoords)
            {
                Destroy(kvp.Value);
            }

            _wallByCoords.Clear();
        }

        private WallView CreateOrGetWall(Vector2Int coords)
        {
            if (_wallByCoords.TryGetValue(coords, out var result))
            {
                return result;
            }
            else
            {
                var wallGo = InstantiatePrefab(PrefabKey.Wall);
                wallGo.transform.position = _gridCalculator.GetCellCenterWorld(coords);
                
                var wallView = wallGo.GetComponent<WallView>();
                _wallByCoords[coords] = wallView;

                return wallView;
            }
        }

        private void SetWallsSprite()
        {
            var wallSprite = _spritesHolder.GetWallSpriteByKey(ShopModel.WallsType);
            
            var shopSize = ShopModel.Size;
            
            for (var x = 0; x < shopSize.x; x++)
            {
                var coords = new Vector2Int(x, -1);
                TrySetWallSprite(coords, wallSprite);
            }
            
            for (var y = 0; y < shopSize.y; y++)
            {
                var coords = new Vector2Int(-1, y);
                TrySetWallSprite(coords, wallSprite);
            }
        }

        private bool HaveTruckGatesOn(int y)
        {
            return ShopModel.ShopObjects.TryGetValue(new Vector2Int(-1, y), out var shopObject) 
                    && shopObject.ShopObjectType == ShopObjectType.TruckPoint;
        }

        private bool TrySetWallSprite(Vector2Int coords, Sprite wallSprite)
        {
            if (_wallByCoords.TryGetValue(coords, out var wallView))
            {
                wallView.SetWallSprite(wallSprite);
                return true;
            }

            return false;
        }
    }
}