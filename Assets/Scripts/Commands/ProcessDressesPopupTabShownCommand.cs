using Events;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;
using Model.Popups;

namespace Commands
{
    public class ProcessDressesPopupTabShownCommand : ICommand<UIDressesPopupTabShownEvent>
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();

        public void Execute(UIDressesPopupTabShownEvent e)
        {
            var uiFlagsModel = _playerModelHolder.PlayerModel.UIFlagsModel;

            switch (e.TabType)
            {
                case PlayerDressesPopupTabType.TopDresses:
                    uiFlagsModel.SetNewTopDressesFlag(false);
                    break;
                case PlayerDressesPopupTabType.BottomDresses:
                    uiFlagsModel.SetNewBottomDressesFlag(false);
                    break;
                case PlayerDressesPopupTabType.Hairs:
                    uiFlagsModel.SetNewHairsFlag(false);
                    break;
                case PlayerDressesPopupTabType.Glasses:
                    uiFlagsModel.SetNewGlassesFlag(false);
                    break;
            }
        }
    }
}