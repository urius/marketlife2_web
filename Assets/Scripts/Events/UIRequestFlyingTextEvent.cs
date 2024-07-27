using UnityEngine;

namespace Events
{
    public struct UIRequestFlyingTextEvent
    {
        public readonly string Text;
        public readonly Vector3 WorldPosition;
        public readonly UIRequestFlyingTextColor Color;

        public UIRequestFlyingTextEvent(string text, Vector3 worldPosition,
            UIRequestFlyingTextColor color = UIRequestFlyingTextColor.Default)
        {
            Text = text;
            WorldPosition = worldPosition;
            Color = color;
        }
    }

    public enum UIRequestFlyingTextColor
    {
        Default,
        White,
        Red,
        Green,
    }
}