using Events;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;

namespace Commands
{
    public class ProcessClosePopupCommand : ICommand<UIRequestClosePopupEvent>
    {
        private readonly IPopupViewModelsHolder _popupViewModelsHolder = Instance.Get<IPopupViewModelsHolder>();
        
        public void Execute(UIRequestClosePopupEvent e)
        {
            var targetPopupViewModel = e.PopupViewModel;
            
            _popupViewModelsHolder.RemovePopup(targetPopupViewModel);
            
            targetPopupViewModel.Dispose();
        }
    }
}