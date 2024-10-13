using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Other;
using UnityEngine;

namespace Holders
{
    [CreateAssetMenu(fileName = "PlayerDressesDataProviderSo", menuName = "ScriptableObjects/PlayerDressesDataProviderSo")]
    public class PlayerDressesDataProviderSo : ScriptableObject, IPlayerDressesDataProvider
    {
        [LabeledArray(nameof(PlayerDressItemData.PrimarySpriteKey))] [SerializeField]
        private PlayerDressItemData[] _topBodyItems;

        [LabeledArray(nameof(PlayerDressItemData.PrimarySpriteKey))] [SerializeField]
        private PlayerDressItemData[] _bottomBodyItems;

        [LabeledArray(nameof(PlayerDressItemData.PrimarySpriteKey))] [SerializeField]
        private PlayerDressItemData[] _hairItems;

        [LabeledArray(nameof(PlayerDressItemData.PrimarySpriteKey))] [SerializeField]
        private PlayerDressItemData[] _glassItems;

        public PlayerDressItemData[] TopBodyItems => _topBodyItems;
        public PlayerDressItemData[] BottomBodyItems => _bottomBodyItems;
        public PlayerDressItemData[] HairAndHatItems => _hairItems;
        public PlayerDressItemData[] GlassItems => _glassItems;

        public PlayerDressItemData[] GetTopBodyItemsByLevel(int level)
        {
            return GetItemsByLevel(_topBodyItems, level);
        }
        
        public PlayerDressItemData[] GetTopBodyItemsForNextLevel(int level)
        {
            return GetItemsForNextLevel(_topBodyItems, level);
        }

        public PlayerDressItemData[] GetBottomBodyItemsByLevel(int level)
        {
            return GetItemsByLevel(_bottomBodyItems, level);
        }
        
        public PlayerDressItemData[] GetBottomBodyItemsForNextLevel(int level)
        {
            return GetItemsForNextLevel(_bottomBodyItems, level);
        }

        public PlayerDressItemData[] GetHairItemsByLevel(int level)
        {
            return GetItemsByLevel(_hairItems, level);
        }
        
        public PlayerDressItemData[] GetHairItemsForNextLevel(int level)
        {
            return GetItemsForNextLevel(_hairItems, level);
        }

        public PlayerDressItemData[] GetGlassItemsByLevel(int level)
        {
            return GetItemsByLevel(_glassItems, level);
        }
        
        public PlayerDressItemData[] GetGlassItemsForNextLevel(int level)
        {
            return GetItemsForNextLevel(_glassItems, level);
        }

        private PlayerDressItemData[] GetItemsByLevel(IReadOnlyCollection<PlayerDressItemData> items, int level)
        {
            var filteredItems = new List<PlayerDressItemData>(items.Count);

            foreach (var wallItem in items)
            {
                if (wallItem.Level <= level)
                {
                    filteredItems.Add(wallItem);
                }
            }

            return filteredItems.ToArray();
        }
        
        private PlayerDressItemData[] GetItemsForNextLevel(IReadOnlyCollection<PlayerDressItemData> items, int level)
        {
            var closestHigherLevel = items
                .Where(wallItem => wallItem.Level > level)
                .Select(wallItem => wallItem.Level)
                .DefaultIfEmpty(0)
                .Min();

            if (closestHigherLevel <= level)
            {
                return Array.Empty<PlayerDressItemData>();
            }

            var targetItems = 
                items.Where(i => i.Level == closestHigherLevel)
                    .ToArray();

            return targetItems;
        }
    }

    [Serializable]
    public class PlayerDressItemData
    {
        public int Level;
        public ManSpriteType PrimarySpriteKey;
        public ManSpriteType SecondarySpriteKey;
    }

    public interface IPlayerDressesDataProvider
    {
        public PlayerDressItemData[] TopBodyItems { get; }
        public PlayerDressItemData[] BottomBodyItems { get; }
        public PlayerDressItemData[] HairAndHatItems { get; }
        public PlayerDressItemData[] GlassItems { get; }
        
        public PlayerDressItemData[] GetTopBodyItemsByLevel(int level);
        public PlayerDressItemData[] GetTopBodyItemsForNextLevel(int level);

        public PlayerDressItemData[] GetBottomBodyItemsByLevel(int level);
        public PlayerDressItemData[] GetBottomBodyItemsForNextLevel(int level);

        public PlayerDressItemData[] GetHairItemsByLevel(int level);
        public PlayerDressItemData[] GetHairItemsForNextLevel(int level);

        public PlayerDressItemData[] GetGlassItemsByLevel(int level);
        public PlayerDressItemData[] GetGlassItemsForNextLevel(int level);
    }
}