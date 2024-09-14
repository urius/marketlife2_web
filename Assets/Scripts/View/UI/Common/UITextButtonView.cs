using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.Common
{
    public class UITextButtonView : MonoBehaviour
    {
        [SerializeField] private Image _buttonImage;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _button;
        [SerializeField] private SkinData _orangeSkinData;
        [SerializeField] private SkinData _crimsonSkinData;
        [SerializeField] private SkinData _greenSkinData;

        public Button Button => _button;
        public TMP_Text Text => _text;
        
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

        public void SetText(string text)
        {
            _text.text = text;
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