using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.BottomPanel
{
    public class UIUpgradeTruckPointPanelView : MonoBehaviour
    {
        public event Action UpgradeButtonClicked;
        public event Action HireStaffButtonClicked;
        
        [SerializeField] private TMP_Text _deliverTitleText;
        [SerializeField] private Image[] _productIcons;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private TMP_Text _upgradeButtonText;
        
        [Space]
        [SerializeField] private TMP_Text _staffTitleText;
        [SerializeField] private Image[] _staffIcons;
        [SerializeField] private TMP_Text[] _staffWorkTimerTexts;
        [SerializeField] private Button _hireStaffButton;
        [SerializeField] private TMP_Text _hireStaffButtonText;

        private void Awake()
        {
            _upgradeButton.onClick.AddListener(UpgradeButtonClickHandler);
            _hireStaffButton.onClick.AddListener(HireStaffButtonClickHandler);
        }

        private void OnDestroy()
        {
            _upgradeButton.onClick.RemoveListener(UpgradeButtonClickHandler);
            _hireStaffButton.onClick.RemoveListener(HireStaffButtonClickHandler);
        }

        private void UpgradeButtonClickHandler()
        {
            UpgradeButtonClicked?.Invoke();
        }

        private void HireStaffButtonClickHandler()
        {
            HireStaffButtonClicked?.Invoke();
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
                _productIcons[productIndex].sprite = iconSprite;
            }
        }
        

        public void SetStaffIconSprite(int slotIndex, Sprite iconSprite)
        {
            if (slotIndex >= 0 && slotIndex < _productIcons.Length)
            {
                _staffIcons[slotIndex].sprite = iconSprite;
            }
        }

        public void SetUpgradeButtonText(string text)
        {
            _upgradeButtonText.text = text;
        }

        public void SetHireStaffButtonText(string text)
        {
            _hireStaffButtonText.text = text;
        }

        public void SetUpgradeButtonEnabledState(bool isEnabled)
        {
            _upgradeButton.enabled = isEnabled;
        }

        public void SetHireStaffButtonEnabledState(bool isEnabled)
        {
            _hireStaffButton.enabled = isEnabled;
        }
        
        public void SetStaffWorkTimerText(int staffIndex, string text)
        {
            if (staffIndex >= 0 && staffIndex < _staffWorkTimerTexts.Length)
            {
                _staffWorkTimerTexts[staffIndex].text = text;
            }
        }

        public void SetStaffSlotEnabled(int slotIndex, bool isEnabled)
        {
            _staffIcons[slotIndex].gameObject.SetActive(isEnabled);
            _staffWorkTimerTexts[slotIndex].gameObject.SetActive(isEnabled);
        }
    }
}