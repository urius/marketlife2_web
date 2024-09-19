using Events;
using Holders;
using Infra.CommandExecutor;
using Infra.Instance;
using Model.Popups;

namespace Commands
{
    public class ShowInteriorPopupCommand : ICommand<UIInteriorButtonClickedEvent>
    {
        private readonly IPopupViewModelsHolder _popupViewModelsHolder = Instance.Get<IPopupViewModelsHolder>();
        public void Execute(UIInteriorButtonClickedEvent e)
        {
            var popupViewModel = new InteriorPopupViewModel();
            
            _popupViewModelsHolder.AddPopup(popupViewModel);
        }
    }
}