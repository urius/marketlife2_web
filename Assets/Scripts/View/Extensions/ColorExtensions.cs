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

        public static Color SetColorWithoutAlpha(this Color currentColor, Color newColor)
        {
            var result = currentColor;
            result.r = newColor.r;
            result.g = newColor.g;
            result.b = newColor.b;

            return result;
        }

        public static void SetAlpha(this Image image, float alpha)
        {
            image.color = image.color.SetAlpha(alpha);
        }

        public static void SetAlpha(this SpriteRenderer spriteRenderer, float alpha)
        {
            spriteRenderer.color = spriteRenderer.color.SetAlpha(alpha);
        }

        public static void SetColorWithoutAlpha(this SpriteRenderer spriteRenderer, Color color)
        {
            spriteRenderer.color = spriteRenderer.color.SetColorWithoutAlpha(color);
        }
    }
}