using System;
using System.Linq;
using Data;
using Other;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Holders
{
    [CreateAssetMenu(fileName = "SpritesHolderSo", menuName = "ScriptableObjects/SpritesHolderSo")]
    public class SpritesHolderSo : ScriptableObject
    {
        [LabeledArray(nameof(SpritesHolderFloorItem.Key))] [SerializeField]
        private SpritesHolderFloorItem[] _floorItems;

        [LabeledArray(nameof(SpritesHolderWallItem.Key))] [SerializeField]
        private SpritesHolderWallItem[] _wallItems;
        
        [LabeledArray(nameof(SpritesHolderProductItem.Key))] [SerializeField]
        private SpritesHolderProductItem[] _productItems;
        
        [LabeledArray(nameof(SpritesHolderManSpriteItem.Key))] [SerializeField]
        private SpritesHolderManSpriteItem[] _manSpriteItems;

        public Tile GetFloorTileByKey(FloorType key)
        {
            return _floorItems.First(i => i.Key == key).Tile;
        }

        public Sprite GetWallSpriteByKey(WallType key)
        {
            return _wallItems.First(i => i.Key == key).Sprite;
        }

        public Sprite GetProductSpriteByKey(ProductType key)
        {
            return _productItems.FirstOrDefault(i => i.Key == key).Sprite;
        }

        public Sprite GetManSpriteByKey(ManSpriteType key)
        {
            return _manSpriteItems.FirstOrDefault(i => i.Key == key).Sprite;
        }

        [Serializable]
        private struct SpritesHolderFloorItem
        {
            public FloorType Key;
            public Tile Tile;
        }

        [Serializable]
        private struct SpritesHolderWallItem
        {
            public WallType Key;
            public Sprite Sprite;
        }

        [Serializable]
        private struct SpritesHolderProductItem
        {
            public ProductType Key;
            public Sprite Sprite;
        }

        [Serializable]
        private struct SpritesHolderManSpriteItem
        {
            public ManSpriteType Key;
            public Sprite Sprite;
        }
    }
}