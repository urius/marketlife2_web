using Data;
using Holders;
using Infra.Instance;
using Providers;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace View.Game.Floors
{
    public class FloorsMediator : MediatorBase
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly SpritesHolderSo _spritesHolder = Instance.Get<SpritesHolderSo>();
        
        private Tilemap _tilemap;

        protected override void MediateInternal()
        {
            _tilemap = TargetTransform.GetComponentInChildren<Tilemap>();
            
            DisplayFloors();
        }

        protected override void UnmediateInternal()
        {
        }

        private void DisplayFloors()
        {
            var shopModel = _playerModelHolder.PlayerModel.ShopModel;
            var floorTile = _spritesHolder.GetFloorTileByKey(shopModel.FloorsType);
            
            for (var x = 0; x < shopModel.Size.x; x++)
            {
                for (var y = 0; y < shopModel.Size.y; y++)
                {
                    _tilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
                }
            }

            var grassGapCells = 5;
            
            for (var x = shopModel.Size.x; x < shopModel.Size.x + grassGapCells; x++)
            {
                for (var y = -grassGapCells; y < shopModel.Size.y + grassGapCells; y++)
                {
                    DrawRandomGrassTile(x, y);
                }
            }
            
            for (var y = shopModel.Size.y; y < shopModel.Size.y + grassGapCells; y++)
            {
                for (var x = -grassGapCells; x < shopModel.Size.x + grassGapCells; x++)
                {
                    DrawRandomGrassTile(x, y);
                }
            }
        }

        private void DrawRandomGrassTile(int x, int y)
        {
            var tile = GetRandomGrassTile();
            var cellPosition = new Vector3Int(x, y, 0);
            var rotation = Random.value < 0.5 ? 0 : 90f;

            _tilemap.SetTile(cellPosition, tile);
            RotateTile(cellPosition, rotation);
        }

        private void RotateTile(Vector3Int cellPosition, float rotation)
        {
            _tilemap.SetTransformMatrix(cellPosition, Matrix4x4.Rotate(Quaternion.Euler(0, 0, rotation)));
        }

        private Tile GetRandomGrassTile()
        {
            var grassType = Random.value < 0.5 ? FloorType.GrassFloor1 : FloorType.GrassFloor2;

            var tile = _spritesHolder.GetFloorTileByKey(grassType);
            
            return tile;
        }
    }
}