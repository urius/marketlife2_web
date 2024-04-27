using System.Collections.Generic;
using Data;
using Infra.Instance;
using Model;
using Providers;
using UnityEngine;
using Utils;

namespace View.Game.Walls
{
    public class WallsMediator : MediatorBase
    {
        private readonly IPlayerModelHolder _playerModelHolder;
        private readonly IGridCalculator _gridCalculator;
        private readonly Dictionary<Vector2Int, WallView> _wallByCoords = new();

        private ShopModel ShopModel => _playerModelHolder.PlayerModel.ShopModel;

        public WallsMediator()
        {
            _playerModelHolder = Instance.Get<IPlayerModelHolder>();
            _gridCalculator = Instance.Get<IGridCalculator>();
        }

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
    }
}