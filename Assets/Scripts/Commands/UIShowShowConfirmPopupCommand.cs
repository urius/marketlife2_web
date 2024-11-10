using Cysharp.Threading.Tasks;
using Data;
using Extensions;
using Infra.CommandExecutor;
using Infra.Instance;
using Tools.AudioManager;
using UnityEngine;
using Utils;
using View.UI.Common;
using View.UI.Popups.YesNoPopup;

namespace Commands
{
    public struct ShowPromptCommandParams
    {
        public readonly Transform TargetTransform;
        public readonly string Title;
        public readonly string Message;
        public readonly string YesButtonText;
        public readonly string NoButtonText;
        public readonly ButtonPresetType YesButtonPreset;
        public readonly ButtonPresetType NoButtonPreset;
        public readonly bool HideCloseButton;

        public ShowPromptCommandParams(
            Transform targetTransform,
            string title,
            string message,
            string yesButtonText,
            string noButtonText, 
            ButtonPresetType yesButtonPreset = ButtonPresetType.Default, 
            ButtonPresetType noButtonPreset = ButtonPresetType.Default,
            bool hideCloseButton = false)
        {
            TargetTransform = targetTransform;
            Title = title;
            Message = message;
            YesButtonText = yesButtonText;
            NoButtonText = noButtonText;
            YesButtonPreset = yesButtonPreset;
            NoButtonPreset = noButtonPreset;
            HideCloseButton = hideCloseButton;
        }
    }

    public enum ButtonPresetType
    {
        Default,
        Orange,
        Crimson,
        Green,
        Blue,
    }
    
    public class UIShowShowConfirmPopupCommand : IAsyncCommandWithResult<bool, ShowPromptCommandParams>
    {
        private readonly UniTaskCompletionSource<bool> _resultTcs = new ();

        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        
        public async UniTask<bool> ExecuteAsync(ShowPromptCommandParams arg)
        {
            var popupView = InstantiateHelper.InstantiatePrefab<UIYesNoPopupView>(PrefabKey.UIYesNoPopup, arg.TargetTransform);

            Setup(popupView, arg);

            _audioPlayer.PlaySound(SoundIdKey.PopupOpen);
            await popupView.Appear2Async();

            Subscribe(popupView);

            var result = await _resultTcs.Task;
            
            Unsubscribe(popupView);
            DisappearAndDestroyPopup(popupView).Forget();
            
            return result;
        }

        private static void Setup(UIYesNoPopupView popupView, ShowPromptCommandParams arg)
        {
            popupView.SetTitleText(arg.Title);
            popupView.SetMessageText(arg.Message);
            if (string.IsNullOrEmpty(arg.YesButtonText))
            {
                popupView.YesButton.SetVisibility(false);
            }
            else
            {
                popupView.YesButton.SetText(arg.YesButtonText);
            }

            if (string.IsNullOrEmpty(arg.NoButtonText))
            {
                popupView.NoButton.SetVisibility(false);

                PuYesButtonOnCenter(popupView.YesButton);
            }
            else
            {
                popupView.NoButton.SetText(arg.NoButtonText);
            }

            popupView.SetCloseButtonVisibility(!arg.HideCloseButton);

            ApplyPreset(popupView.YesButton, arg.YesButtonPreset);
            ApplyPreset(popupView.NoButton, arg.NoButtonPreset);
        }

        private static void PuYesButtonOnCenter(UITextButtonView yesButton)
        {
            var yesButtonRectTransform = yesButton.RectTransform;
            var pos = yesButtonRectTransform.anchoredPosition;
            pos.x = 0;
            yesButtonRectTransform.anchoredPosition = pos;
            yesButtonRectTransform.pivot = new Vector2(0.5f, yesButtonRectTransform.pivot.y);
            yesButtonRectTransform.anchorMax = new Vector2(0.5f, yesButtonRectTransform.anchorMax.y);
            yesButtonRectTransform.anchorMin = new Vector2(0.5f, yesButtonRectTransform.anchorMin.y);
        }

        private static void ApplyPreset(UITextButtonView button, ButtonPresetType preset)
        {
            switch (preset)
            {
                case ButtonPresetType.Orange:
                    button.SetOrangeSkinData();
                    break;
                case ButtonPresetType.Crimson:
                    button.SetCrimsonSkinData();
                    break;
                case ButtonPresetType.Green:
                    button.SetGreenSkinData();
                    break;
                case ButtonPresetType.Blue:
                    button.SetBlueSkinData();
                    break;
            }
        }

        private void Subscribe(UIYesNoPopupView popupView)
        {
            popupView.YesButton.ButtonClicked += OnYes;
            popupView.NoButton.ButtonClicked += OnCancel;
            popupView.CloseButtonClicked += OnCancel;
        }

        private void Unsubscribe(UIYesNoPopupView popupView)
        {
            popupView.YesButton.ButtonClicked -= OnYes;
            popupView.NoButton.ButtonClicked -= OnCancel;
            popupView.CloseButtonClicked -= OnCancel;
        }

        private void OnYes()
        {
            _resultTcs.TrySetResult(true);
        }

        private void OnCancel()
        {
            _resultTcs.TrySetResult(false);
        }

        private async UniTask DisappearAndDestroyPopup(UIYesNoPopupView popupView)
        {
            _audioPlayer.PlaySound(SoundIdKey.PopupClose);
            await popupView.Disappear2Async();
            
            InstantiateHelper.Destroy(popupView);
        }
    }
}