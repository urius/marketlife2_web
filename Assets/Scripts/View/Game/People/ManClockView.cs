using UnityEngine;

namespace View.Game.People
{
    public class ManClockView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public void SetIconColor(Color color)
        {
            _spriteRenderer.color = color;
        }
    }
}