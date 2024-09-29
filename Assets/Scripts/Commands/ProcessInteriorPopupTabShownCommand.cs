using Data.Internal;
using Events;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;

namespace Commands
{
    public class ProcessInteriorPopupTabShownCommand : ICommand<UIInteriorPopupTabShownEvent>
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        public void Execute(UIInteriorPopupTabShownEvent e)
        {
            var uiFlagsModel = _playerModelHolder.PlayerModel.UIFlagsModel;

            switch (e.TabType)
            {
                case InteriorItemType.Floor:
                    uiFlagsModel.SetNewFloorsFlag(false);
                    break;
                case InteriorItemType.Wall:
                    uiFlagsModel.SetNewWallsFlag(false);
                    break;
            }
        }
    }
}