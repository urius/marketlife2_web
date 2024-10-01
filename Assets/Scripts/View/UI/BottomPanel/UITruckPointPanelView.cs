using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.UI.Common;

namespace View.UI.BottomPanel
{
    public class UITruckPointPanelView : UIBottomPanelViewBase, IUITruckPointPanelTransformsProvider
    {
        public event Action UpgradeButtonClicked;
        public event Action HireStaffButtonClicked;

        [SerializeField] private TMP_Text _deliverTitleText;
        [SerializeField] private Image[] _productIcons;
        [SerializeField] private Sprite _unknownProductSprite;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private TMP_Text _upgradeButtonText;
        
        [Space]
        [SerializeField] private TMP_Text _staffTitleText;
        [SerializeField] private Image _staffIcon;
        [SerializeField] private Image _clockIcon;
        [SerializeField] private TMP_Text _staffWorkTimerText;
        [SerializeField] private UITextButtonView _hireStaffButtonView;

        public int ProductIconsAmount => _productIcons.Length;
        public RectTransform UpgradeButtonTransform => _upgradeButton.transform as RectTransform;
        public UITextButtonView HireStaffButtonView => _hireStaffButtonView;
        
        private Button HireStaffButton => _hireStaffButtonView.Button;

        protected override void Awake()
        {
            base.Awake();
            
            _upgradeButton.onClick.AddListener(UpgradeButtonClickHandler);
            HireStaffButton.onClick.AddListener(HireStaffButtonClickHandler);
        }

        private void OnDestroy()
        {
            _upgradeButton.onClick.RemoveListener(UpgradeButtonClickHandler);
            HireStaffButton.onClick.RemoveListener(HireStaffButtonClickHandler);
        }

        public void SetDeliverTitleText(string text)
        {
            _deliverTitleText.text = text;
        }

        public void SetStaffTitleText(string text)
        {
            _staffTitleText.text = text;
        }

        public void SetProductIconSprite(int productIndex, Sprite iconSprite)
        {
            if (productIndex >= 0 && productIndex < _productIcons.Length)
            {
                _productIcons[productIndex].sprite = iconSprite != null ? iconSprite : _unknownProductSprite;
            }
        }

        public void SetStaffEnabled(bool isEnabled)
        {
            _staffIcon.enabled = isEnabled;
            _clockIcon.enabled = isEnabled;
            _staffWorkTimerText.enabled = isEnabled;
        }

        public void SetClockColor(Color color)
        {
            _clockIcon.color = color;
        }

        public void SetUpgradeButtonText(string text)
        {
            _upgradeButtonText.text = text;
        }
        
        public void SetUpgradeEnabledState(bool isEnabled)
        {
            _upgradeButton.interactable = isEnabled;
            
            var color = _upgradeButtonText.color;
            color.a = isEnabled ? 1f : 0.7f;
            _upgradeButtonText.color = color;
        }

        public void SetHireStaffButtonInteractable(bool isInteractable)
        {
            _hireStaffButtonView.SetInteractable(isInteractable);
        }

        public void SetStaffWorkTimerText(string text)
        {
            _staffWorkTimerText.text = text;
        }

        public void SetStaffWorkTimerTextColor(Color color)
        {
            _staffWorkTimerText.color = color;
        }

        private void UpgradeButtonClickHandler()
        {
            UpgradeButtonClicked?.Invoke();
        }

        private void HireStaffButtonClickHandler()
        {
            HireStaffButtonClicked?.Invoke();
        }
    }

    public interface IUITruckPointPanelTransformsProvider
    {
        public RectTransform UpgradeButtonTransform { get; }
    }
}