using UnityEngine;
using UnityEngine.UI;

namespace View.Extensions
{
    public static class ColorExtensions
    {
        public static Color SetAlpha(this Color color, float alpha)
        {
            var result = color;
            result.a = alpha;
            
            return result;
        }
        
        public static void SetAlpha(this Image image, float alpha)
        {
            image.color = image.color.SetAlpha(alpha);
        }
    }
}