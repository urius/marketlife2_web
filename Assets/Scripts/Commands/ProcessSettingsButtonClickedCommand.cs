using Data;
using Events;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;
using Model.Popups;
using Tools;

namespace Commands
{
    public class ProcessSettingsButtonClickedCommand : ICommand<UISettingsButtonClickedEvent>
    {
        private readonly IPopupViewModelsHolder _popupViewModelsHolder = Instance.Get<IPopupViewModelsHolder>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        
        public void Execute(UISettingsButtonClickedEvent e)
        {
            if (_popupViewModelsHolder.FindPopupByKey(PopupKey.SettingsPopup) == null)
            {
                var audioSettingsModel = _playerModelHolder.PlayerModel.AudioSettingsModel;
                var playerId = GamePushWrapper.GetPlayerId();
                
                var settingsPopupViewModel = new SettingsPopupViewModel(audioSettingsModel, playerId);
                _popupViewModelsHolder.AddPopup(settingsPopupViewModel);
            }
        }
    }
}