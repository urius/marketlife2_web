using System.Runtime.InteropServices;
using Commands;
using Cysharp.Threading.Tasks;
using Data;
using Events;
using Extensions;
using Holders;
using Infra.CommandExecutor;
using Infra.EventBus;
using Infra.Instance;
using Model.Popups;
using Tools.AudioManager;
using UnityEngine;

namespace View.UI.Popups.SettingsPopup
{
    public class UISettingsPopupController : MediatorWithModelBase<SettingsPopupViewModel>
    {
        [DllImport("__Internal")]
        private static extern void ReloadPage();
        
        private readonly ISharedViewsDataHolder _sharedViewsDataHolder = Instance.Get<ISharedViewsDataHolder>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly ICommandExecutor _commandExecutor = Instance.Get<ICommandExecutor>();
        
        private Transform _overridenTargetTransform;
        private UISettingsPopupView _popupView;

        protected override void MediateInternal()
        {
            _overridenTargetTransform = _sharedViewsDataHolder.GetSettingsCanvasView().transform;

            _popupView = InstantiatePrefab<UISettingsPopupView>(PrefabKey.UISettingsPopup, _overridenTargetTransform);

            UpdateTexts();
            UpdateValues();
            
            Subscribe();
            
            Appear().Forget();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            Destroy(_popupView);
            _popupView = null;
        }

        private void Subscribe()
        {
            _popupView.CloseButtonClicked += OnCloseButtonClicked;
            _popupView.SoundsOption.ToggleValueChanged += OnSoundOptionToggleChanged;
            _popupView.MusicOption.ToggleValueChanged += OnMusicOptionToggleChanged;
            _popupView.ResetPlayerDataButton.ButtonClicked += OnResetPlayerDataButtonClicked;
        }

        private void Unsubscribe()
        {
            _popupView.CloseButtonClicked -= OnCloseButtonClicked;
            _popupView.SoundsOption.ToggleValueChanged -= OnSoundOptionToggleChanged;
            _popupView.MusicOption.ToggleValueChanged -= OnMusicOptionToggleChanged;
            _popupView.ResetPlayerDataButton.ButtonClicked -= OnResetPlayerDataButtonClicked;
        }

        private void OnResetPlayerDataButtonClicked()
        {
            ShowAndProcessResetDataPrompt().Forget();
        }

        private async UniTaskVoid ShowAndProcessResetDataPrompt()
        {
            var title = _localizationProvider.GetLocale(Constants.LocalizationKeyYesNoPopupDefaultTitle);
            var message = _localizationProvider.GetLocale(Constants.LocalizationKeyYesNoPopupResetDataMessage);
            var yesButtonText = _localizationProvider.GetLocale(Constants.LocalizationKeyYes);
            var noButtonText = _localizationProvider.GetLocale(Constants.LocalizationKeyNo);

            var promptResult =
                await _commandExecutor.ExecuteAsync<UIShowShowConfirmPopupCommand, bool, ShowPromptCommandParams>(
                    new ShowPromptCommandParams(
                        _overridenTargetTransform,
                        title,
                        message,
                        yesButtonText,
                        noButtonText,
                        yesButtonPreset: ButtonPresetType.Orange,
                        noButtonPreset: ButtonPresetType.Green));

            if (promptResult)
            {
                _commandExecutor.Execute<ResetPlayerDataCommand>();
                
                title = _localizationProvider.GetLocale(Constants.LocalizationKeySuccess);
                message = _localizationProvider.GetLocale(Constants.LocalizationKeyResetDataSuccessMessage);
                yesButtonText = _localizationProvider.GetLocale(Constants.LocalizationKeyResetDataReloadButtonText);

                var confirmRelaunch = await _commandExecutor
                    .ExecuteAsync<UIShowShowConfirmPopupCommand, bool, ShowPromptCommandParams>(
                        new ShowPromptCommandParams(
                            _overridenTargetTransform,
                            title,
                            message,
                            yesButtonText,
                            noButtonText: null,
                            hideCloseButton: true));
                
                if (confirmRelaunch)
                {
                    ReloadPage();
                }
            }
        }

        private void OnSoundOptionToggleChanged(bool isOn)
        {
            TargetModel.HandleSoundsToggleChanged(isOn);
        }

        private void OnMusicOptionToggleChanged(bool isOn)
        {
            TargetModel.HandleMusicToggleChanged(isOn);
        }

        private void UpdateTexts()
        {
            var title = _localizationProvider.GetLocale(Constants.LocalizationKeySettingsPopupTitle);
            _popupView.SetTitleText(title);

            var optionText = _localizationProvider.GetLocale(Constants.LocalizationKeySettingsPopupSounds);
            _popupView.SoundsOption.SetText(optionText);
            optionText = _localizationProvider.GetLocale(Constants.LocalizationKeySettingsPopupMusic);
            _popupView.MusicOption.SetText(optionText);

            var resetButtonText = _localizationProvider.GetLocale(Constants.LocalizationKeySettingsPopupResetButton); 
            _popupView.ResetPlayerDataButton.SetText(resetButtonText);

            var idText = _localizationProvider.GetLocale(Constants.LocalizationKeySettingsPopupId);
            _popupView.SetIdText(idText + TargetModel.PlayerId);
        }

        private void UpdateValues()
        {
            _popupView.SoundsOption.SetToggleState(!TargetModel.AudioSettingsModel.IsSoundsMuted);
            _popupView.MusicOption.SetToggleState(!TargetModel.AudioSettingsModel.IsMusicMuted);
        }

        private async UniTaskVoid Appear()
        {
            _eventBus.Dispatch(new RequestGamePauseEvent(nameof(UISettingsPopupController), true));
            
            _audioPlayer.PlaySound(SoundIdKey.PopupOpen);
            
            await _popupView.AppearAsync();
        }

        private void OnCloseButtonClicked()
        {
            DisappearAndRequestUnmediate().Forget();
        }
        
        private async UniTaskVoid DisappearAndRequestUnmediate()
        {
            _eventBus.Dispatch(new RequestGamePauseEvent(nameof(UISettingsPopupController), false));
            
            _audioPlayer.PlaySound(SoundIdKey.PopupClose);

            await _popupView.DisappearAsync();
            
            _eventBus.Dispatch(new UIRequestClosePopupEvent(TargetModel));
        }
    }
}