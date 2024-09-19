using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Other;
using UnityEngine;

namespace Holders
{
    [CreateAssetMenu(fileName = "InteriorDataProviderSo", menuName = "ScriptableObjects/InteriorDataProviderSo")]
    public class InteriorDataProviderSo : ScriptableObject, IInteriorDataProvider
    {
        [LabeledArray(nameof(InteriorDataWallItem.WallType))]
        [SerializeField] private InteriorDataWallItem[] _wallItems;
        
        [LabeledArray(nameof(InteriorDataFloorItem.FloorType))]
        [SerializeField] private InteriorDataFloorItem[] _floorItems;

        public InteriorDataWallItem[] GetWallItemsByLevel(int level)
        {
            var filteredItems = new List<InteriorDataWallItem>(_wallItems.Length);

            foreach (var wallItem in _wallItems)
            {
                if (wallItem.Level <= level)
                {
                    filteredItems.Add(wallItem);
                }
            }

            return filteredItems.ToArray();
        }
        
        public InteriorDataWallItem[] GetWallItemsForNextLevel(int level)
        {
            var closestHigherLevel = _wallItems
                .Where(wallItem => wallItem.Level > level)
                .Select(wallItem => wallItem.Level)
                .DefaultIfEmpty(0)
                .Min();

            if (closestHigherLevel <= level)
            {
                return Array.Empty<InteriorDataWallItem>();
            }

            var targetItems = 
                _wallItems.Where(i => i.Level == closestHigherLevel)
                .ToArray();

            return targetItems;
        }
        
        
        public InteriorDataFloorItem[] GetFloorItemsByLevel(int level)
        {
            var filteredItems = new List<InteriorDataFloorItem>(_floorItems.Length);

            foreach (var floorItem in _floorItems)
            {
                if (floorItem.Level <= level)
                {
                    filteredItems.Add(floorItem);
                }
            }

            return filteredItems.ToArray();
        }
        
        public InteriorDataFloorItem[] GetFloorItemsForNextLevel(int level)
        {
            var closestHigherLevel = _floorItems
                .Where(floorItem => floorItem.Level > level)
                .Select(floorItem => floorItem.Level)
                .DefaultIfEmpty(0)
                .Min();

            if (closestHigherLevel <= level)
            {
                return Array.Empty<InteriorDataFloorItem>();
            }

            var targetItems = 
                _floorItems.Where(i => i.Level == closestHigherLevel)
                    .ToArray();

            return targetItems;
        }
    }


    [Serializable]
    public class InteriorDataWallItem
    {
        public WallType WallType;
        public int Level;
    }

    [Serializable]
    public class InteriorDataFloorItem
    {
        public FloorType FloorType;
        public int Level;
    }

    public interface IInteriorDataProvider
    {
        public InteriorDataWallItem[] GetWallItemsByLevel(int level);
        public InteriorDataWallItem[] GetWallItemsForNextLevel(int level);

        public InteriorDataFloorItem[] GetFloorItemsByLevel(int level);
        public InteriorDataFloorItem[] GetFloorItemsForNextLevel(int level);

    }
}