using Events;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;

namespace Commands
{
    public class ProcessClosePopupClickedCommand : ICommand<UIClosePopupClickedEvent>
    {
        private readonly IPopupViewModelsHolder _popupViewModelsHolder = Instance.Get<IPopupViewModelsHolder>();
        
        public void Execute(UIClosePopupClickedEvent e)
        {
            var targetPopupViewModel = e.PopupViewModel;
            
            _popupViewModelsHolder.RemovePopup(targetPopupViewModel);
        }
    }
}