using System;
using Data;

namespace Model.Popups
{
    public class SettingsPopupViewModel : PopupViewModelBase
    {
        public event Action<bool> SoundsMuteStateChanged
        {
            add => AudioSettingsModel.SoundsMutedStateChanged += value;
            remove => AudioSettingsModel.SoundsMutedStateChanged -= value;
        }
        
        public event Action<bool> MusicMuteStateChanged
        {
            add => AudioSettingsModel.MusicMutedStateChanged += value;
            remove => AudioSettingsModel.MusicMutedStateChanged -= value;
        }


        public readonly PlayerAudioSettingsModel AudioSettingsModel;
        public readonly string PlayerId;

        public SettingsPopupViewModel(PlayerAudioSettingsModel audioSettingsModel, string playerId)
        {
            AudioSettingsModel = audioSettingsModel;
            PlayerId = playerId;
        }
        
        public override PopupKey PopupKey => PopupKey.SettingsPopup;

        //experimental architecture: change model via mediator->view model->model (getting rid of extra layers, like event->command)
        public void HandleSoundsToggleChanged(bool isOn)
        {
            AudioSettingsModel.SetSoundsMuted(!isOn);
        }
        
        //experimental architecture
        public void HandleMusicToggleChanged(bool isOn)
        {
            AudioSettingsModel.SetMusicMuted(!isOn);
        }

        public override void Dispose()
        {
        }
    }
}