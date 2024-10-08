using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.Common
{
    public class UITextButtonView : MonoBehaviour
    {
        public event Action ButtonClicked;
            
        [SerializeField] private Image _buttonImage;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _button;
        [Space(25)]
        [SerializeField] private SkinData _orangeSkinData;
        [SerializeField] private SkinData _crimsonSkinData;
        [SerializeField] private SkinData _greenSkinData;
        [SerializeField] private SkinData _blueSkinData;

        public RectTransform RectTransform => transform as RectTransform;
        public Button Button => _button;
        public TMP_Text Text => _text;

        private void Awake()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void OnButtonClick()
        {
            ButtonClicked?.Invoke();
        }

        public void SetOrangeSkinData()
        {
            ApplySkinData(_orangeSkinData);
        }
        
        public void SetCrimsonSkinData()
        {
            ApplySkinData(_crimsonSkinData);
        }
        
        public void SetGreenSkinData()
        {
            ApplySkinData(_greenSkinData);
        }
        
        public void SetBlueSkinData()
        {
            ApplySkinData(_blueSkinData);
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetVisibility(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
        
        private void ApplySkinData(SkinData skinData)
        {
            _buttonImage.sprite = skinData.SkinSprite;
            _text.color = skinData.TextColor;
            UpdateTextAlpha();
        }
        

        [Serializable]
        private struct SkinData
        {
            public Sprite SkinSprite;
            public Color TextColor;
        }

        public void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;

            UpdateTextAlpha();
        }

        private void UpdateTextAlpha()
        {
            var color = _text.color;
            color.a = _button.interactable ? 1f : 0.7f;
            _text.color = color;
        }
    }
}