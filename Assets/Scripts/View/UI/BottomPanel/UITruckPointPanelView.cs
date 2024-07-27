using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.BottomPanel
{
    public class UITruckPointPanelView : MonoBehaviour
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
        [SerializeField] private TMP_Text[] _staffWorkTimerTexts;
        [SerializeField] private Button _hireStaffButton;
        [SerializeField] private TMP_Text _hireStaffButtonText;

        
        private RectTransform _rectTransform;
        private float _slideDownYPosition;

        public int ProductIconsAmount => _productIcons.Length;
        
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _slideDownYPosition = _rectTransform.anchoredPosition.y;
            
            _upgradeButton.onClick.AddListener(UpgradeButtonClickHandler);
            _hireStaffButton.onClick.AddListener(HireStaffButtonClickHandler);
        }

        private void OnDestroy()
        {
            _upgradeButton.onClick.RemoveListener(UpgradeButtonClickHandler);
            _hireStaffButton.onClick.RemoveListener(HireStaffButtonClickHandler);
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public float SetSlideUpPositionPercent(float positionPercent)
        {
            var yPos = Mathf.Lerp(_slideDownYPosition, 0, positionPercent);
            _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, yPos);

            return yPos;
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
        
        public void SetUpgradeEnabledState(bool isEnabled)
        {
            _upgradeButton.interactable = isEnabled;
            
            var color = _upgradeButtonText.color;
            color.a = isEnabled ? 1f : 0.7f;
            _upgradeButtonText.color = color;
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

        private void UpgradeButtonClickHandler()
        {
            UpgradeButtonClicked?.Invoke();
        }

        private void HireStaffButtonClickHandler()
        {
            HireStaffButtonClicked?.Invoke();
        }
    }
}