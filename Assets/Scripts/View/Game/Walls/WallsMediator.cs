using System.Collections.Generic;
using Data;
using Holders;
using Infra.Instance;
using Model;
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
        }

        protected override void UnmediateInternal()
        {
            RemoveWalls();
        }

        private void DisplayWalls()
        {
            var shopSize = ShopModel.Size;
            var wallSprite = _spritesHolder.GetWallSpriteByKey(ShopModel.WallsType);
            
            for (var x = 0; x < shopSize.x; x++)
            {
                var coords = new Vector2Int(x, -1);
                var wallView = CreateWallIfNeeded(coords);
                wallView.ToXMode(x == shopSize.x - 1);
            }
            
            for (var y = 0; y < shopSize.y; y++)
            {
                var coords = new Vector2Int(-1, y);
                var wallView = CreateWallIfNeeded(coords);
                wallView.ToYMode(y == shopSize.y - 1);
            }

            SetWallsSprite(wallSprite);
        }

        private void RemoveWalls()
        {
            foreach (var kvp in _wallByCoords)
            {
                Object.Destroy(kvp.Value.gameObject);
            }

            _wallByCoords.Clear();
        }

        private WallView CreateWallIfNeeded(Vector2Int coords)
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

        private void SetWallsSprite(Sprite sprite)
        {
            var shopSize = ShopModel.Size;
            
            for (var x = 0; x < shopSize.x; x++)
            {
                var coords = new Vector2Int(x, -1);
                _wallByCoords[coords].SetWallSprite(sprite);
            }
            
            for (var y = 0; y < shopSize.y; y++)
            {
                var coords = new Vector2Int(-1, y);
                _wallByCoords[coords].SetWallSprite(sprite);
            }
        }
    }
}