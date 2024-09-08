using System;
using UnityEngine;

namespace View.Game.People
{
    public static class StaffCharHelper
    {
        private static readonly Color[] TimeProgressColors = { Color.red, Color.yellow, Color.yellow, Color.yellow };

        public static Color GetClockColorByPercent(float percent)
        {
            if (percent >= 1)
            {
                return Color.green;
            }
            
            var colorIndex = percent * (TimeProgressColors.Length - 1);
            var index = (int)colorIndex;
        
            index = Math.Clamp(index, 0, TimeProgressColors.Length - 1);
            
            return TimeProgressColors[index];
        }
    }
}