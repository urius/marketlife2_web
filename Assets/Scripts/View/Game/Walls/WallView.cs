using System;
using UnityEngine;

namespace View.Game.Walls
{
    public class WallView : MonoBehaviour
    {
        [SerializeField] private GameObject _edgeHorizontal;
        [SerializeField] private GameObject _edgeVerticalLeft;
        [SerializeField] private GameObject _edgeVerticalRight;
        [SerializeField] private SpriteRenderer _wallSpriteRenderer;

        public void ToXMode(bool isLast)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 180);
            
            _edgeVerticalLeft.SetActive(false);
            _edgeVerticalRight.SetActive(isLast);
        }
        
        public void ToYMode(bool isLast)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 90);
            
            _edgeVerticalLeft.SetActive(isLast);
            _edgeVerticalRight.SetActive(false);
        }
    }
}