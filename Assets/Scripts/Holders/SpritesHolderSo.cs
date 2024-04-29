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

        [LabeledArray(nameof(SpritesHolderFloorItem.Key))] [SerializeField]
        private SpritesHolderWallItem[] _wallItems;

        public Tile GetFloorTileByKey(FloorType key)
        {
            return _floorItems.First(i => i.Key == key).Tile;
        }

        public Sprite GetWallSpriteByKey(WallType key)
        {
            return _wallItems.First(i => i.Key == key).Sprite;
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
    }
}