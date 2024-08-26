using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.UI.Common;

namespace View.UI.BottomPanel
{
    public class UICashDeskPanelView : UIBottomPanelViewBase
    {
        public event Action HireStaffButtonClicked;
        
        [SerializeField] private TMP_Text _staffTitleText;
        [SerializeField] private Image _staffIcon;
        [SerializeField] private Image _staffProgressIcon;
        [SerializeField] private TMP_Text _staffWorkTimerText;
        [SerializeField] private UITextButtonView _hireStaffButtonView;

        public UITextButtonView HireStaffButtonView => _hireStaffButtonView;
        
        private Button HireStaffButton => _hireStaffButtonView.Button;

        protected override void Awake()
        {
            base.Awake();
            
            HireStaffButton.onClick.AddListener(HireStaffButtonClickHandler);
        }

        private void OnDestroy()
        {
            HireStaffButton.onClick.RemoveListener(HireStaffButtonClickHandler);
        }

        public void SetStaffTitleText(string text)
        {
            _staffTitleText.text = text;
        }
        
        public void SetStaffEnabled(bool isEnabled)
        {
            _staffIcon.enabled = isEnabled;
            _staffProgressIcon.gameObject.SetActive(isEnabled);
            _staffWorkTimerText.enabled = isEnabled;
        }

        public void SetHireStaffButtonText(string text)
        {
            _hireStaffButtonView.Text.text = text;
        }
        
        public void SetHireStaffButtonInteractable(bool isInteractable)
        {
            _hireStaffButtonView.SetInteractable(isInteractable);
        }

        public void SetStaffWorkTimerText(string text)
        {
            _staffWorkTimerText.text = text;
        }
        
        public void SetStaffWorkTimeProgress(float progress)
        {
            _staffProgressIcon.fillAmount = progress;
        }

        private void HireStaffButtonClickHandler()
        {
            HireStaffButtonClicked?.Invoke();
        }
    }
}