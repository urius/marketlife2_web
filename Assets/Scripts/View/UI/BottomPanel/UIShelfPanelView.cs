using System;
using TMPro;
using UnityEngine;
using View.UI.Common;

namespace View.UI.BottomPanel
{
    public class UIShelfPanelView : UIBottomPanelViewBase
    {
        public event Action UpgradeButtonClicked;
        
        [SerializeField] private TMP_Text _shelfTitleText;
        [SerializeField] private UITextButtonView _upgradeButton;

        public UITextButtonView UpgradeButton => _upgradeButton;
        public TMP_Text UpgradeButtonText => _upgradeButton.Text;

        protected override void Awake()
        {
            base.Awake();
            
            _upgradeButton.Button.onClick.AddListener(UpgradeButtonClickHandler);
        }

        public void SetTitleText(string text)
        {
            _shelfTitleText.text = text;
        }

        private void UpgradeButtonClickHandler()
        {
            UpgradeButtonClicked?.Invoke();
        }
    }
}