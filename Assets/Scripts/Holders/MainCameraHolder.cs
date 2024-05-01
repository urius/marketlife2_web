using UnityEngine;

namespace Holders
{
    public class MainCameraHolder : IMainCameraHolder
    {
        public Camera MainCamera { get; }

        public MainCameraHolder(Camera mainCamera)
        {
            MainCamera = mainCamera;
        }
    }

    public interface IMainCameraHolder
    {
        public Camera MainCamera { get; }
    }
}