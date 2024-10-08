using System;
using UnityEngine;
using View.UI.Common;

namespace View.UI.SettingsCanvas
{
    public class UISettingsCanvasView : MonoBehaviour
    {
        public event Action SettingsButtonClicked
        {
            add => _settingsButton.Clicked += value;
            remove => _settingsButton.Clicked -= value;
        }

        [SerializeField] private UISimpleButtonView _settingsButton;
    }
}