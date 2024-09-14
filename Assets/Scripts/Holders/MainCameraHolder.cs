using System;
using UnityEngine;

namespace Holders
{
    public class MainCameraHolder : IMainCameraHolder, IPlayerFocusProvider, IPlayerFocusSetter
    {
        public event Action<bool> PlayerFocusChanged; 
        
        public Camera MainCamera { get; }
        public bool IsPlayerFocused { get; private set; }

        public MainCameraHolder(Camera mainCamera)
        {
            MainCamera = mainCamera;
        }

        public void SetPlayerFocusedFlag(bool isFocused)
        {
            if (IsPlayerFocused == isFocused) return;
            
            IsPlayerFocused = isFocused;
            PlayerFocusChanged?.Invoke(isFocused);
        }
    }

    public interface IMainCameraHolder
    {
        public Camera MainCamera { get; }
    }

    public interface IPlayerFocusProvider
    {
        bool IsPlayerFocused { get; }
        event Action<bool> PlayerFocusChanged;
    }
    
    public interface IPlayerFocusSetter
    {
        void SetPlayerFocusedFlag(bool isFocused);
    }
}