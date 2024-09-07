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
        [SerializeField] private TMP_Text _staffWorkTimerText;
        [SerializeField] private UITextButtonView _hireStaffButtonView;

        public UITextButtonView HireStaffButtonView => _hireStaffButtonView;

        protected override void Awake()
        {
            base.Awake();
            
            _hireStaffButtonView.Button.onClick.AddListener(HireStaffButtonClickHandler);
        }

        private void OnDestroy()
        {
            _hireStaffButtonView.Button.onClick.RemoveListener(HireStaffButtonClickHandler);
        }

        public void SetStaffTitleText(string text)
        {
            _staffTitleText.text = text;
        }
        
        public void SetStaffEnabled(bool isEnabled)
        {
            _staffIcon.enabled = isEnabled;
            _staffWorkTimerText.enabled = isEnabled;
        }
        
        public void SetHireStaffButtonInteractable(bool isInteractable)
        {
            _hireStaffButtonView.SetInteractable(isInteractable);
        }

        public void SetStaffWorkTimerText(string text)
        {
            _staffWorkTimerText.text = text;
        }

        private void HireStaffButtonClickHandler()
        {
            HireStaffButtonClicked?.Invoke();
        }
    }
}