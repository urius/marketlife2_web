using Infra.Instance;
using UnityEngine;

namespace Holders
{
    public class ScreenCalculator : IScreenCalculator
    {
        private readonly IMainCameraHolder _mainCameraHolder = Instance.Get<IMainCameraHolder>();
        
        private Camera MainCamera => _mainCameraHolder.MainCamera;


        public Vector3 ScreenToWorldPoint(Vector2 screenPoint)
        {
            return MainCamera.ScreenToWorldPoint(screenPoint);
        }
        
        public Vector3 GetWorldMousePoint()
        {
            return MainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public interface IScreenCalculator
    {
        public Vector3 ScreenToWorldPoint(Vector2 screenPoint);
        public Vector3 GetWorldMousePoint();
    }
}