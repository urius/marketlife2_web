using UnityEngine;

namespace View.Game.People
{
    public static class StaffCharHelper
    {
        private static readonly Color[] TimeProgressColors = { Color.red, Color.yellow, Color.yellow, Color.yellow, Color.green,Color.green, Color.green };

        public static Color GetClockColorByPercent(float percent)
        {
            var colorIndex = percent * (TimeProgressColors.Length - 1);
            var index = (int)(colorIndex);
        
            return TimeProgressColors[index];
        }
    }
}