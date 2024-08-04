using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.UI.Common;

namespace View.UI.BottomPanel
{
    public class UITruckPointPanelView : UIBottomPanelViewBase
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
        [SerializeField] private Image[] _staffIcons;
        [SerializeField] private Image[] _staffProgressIcons;
        [SerializeField] private TMP_Text[] _staffWorkTimerTexts;
        [SerializeField] private UITextButtonView _hireStaffButtonView;

        public int ProductIconsAmount => _productIcons.Length;
        public Transform UpgradeButtonTransform => _upgradeButton.transform;
        public UITextButtonView HireStaffButtonView => _hireStaffButtonView;
        
        private Button HireStaffButton => _hireStaffButtonView.Button;
        private TMP_Text HireStaffButtonText => _hireStaffButtonView.Text;

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

        public void SetStaffEnabled(int slotIndex, bool isEnabled)
        {
            _staffIcons[slotIndex].enabled = isEnabled;
            _staffProgressIcons[slotIndex].gameObject.SetActive(isEnabled);
            _staffWorkTimerTexts[slotIndex].enabled = isEnabled;
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

        public void SetHireStaffButtonText(string text)
        {
            HireStaffButtonText.text = text;
        }

        public void SetHireStaffButtonInteractable(bool isInteractable)
        {
            _hireStaffButtonView.SetInteractable(isInteractable);
        }
        
        public void SetStaffWorkTimerText(int staffIndex, string text)
        {
            if (staffIndex >= 0 && staffIndex < _staffWorkTimerTexts.Length)
            {
                _staffWorkTimerTexts[staffIndex].text = text;
            }
        }

        private void UpgradeButtonClickHandler()
        {
            UpgradeButtonClicked?.Invoke();
        }

        private void HireStaffButtonClickHandler()
        {
            HireStaffButtonClicked?.Invoke();
        }

        public void SetStaffWorkTimeProgress(int i, float progress)
        {
            _staffProgressIcons[i].fillAmount = progress;
        }
    }
}