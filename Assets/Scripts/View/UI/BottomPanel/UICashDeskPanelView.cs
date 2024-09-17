using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.UI.Common;

namespace View.UI.BottomPanel
{
    public class UICashDeskPanelView : UIBottomPanelViewBase, IUICashDeskPanelTransformsProvider
    {
        public event Action HireStaffButtonClicked;
        
        [SerializeField] private TMP_Text _staffTitleText;
        [SerializeField] private Image _staffIcon;
        [SerializeField] private Image _clockIcon;
        [SerializeField] private TMP_Text _staffWorkTimerText;
        [SerializeField] private UITextButtonView _hireStaffButtonView;

        public UITextButtonView HireStaffButtonView => _hireStaffButtonView;

        public RectTransform HireButtonTransform => _hireStaffButtonView.transform as RectTransform;

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
            _clockIcon.enabled = isEnabled;
            _staffWorkTimerText.enabled = isEnabled;
        }

        public void SetClockColor(Color color)
        {
            _clockIcon.color = color;
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

    public interface IUICashDeskPanelTransformsProvider
    {
        public RectTransform HireButtonTransform { get; }
    }
}