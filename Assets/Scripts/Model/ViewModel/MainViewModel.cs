using System;

namespace Model.ViewModel
{
    public class MainViewModel : IMainViewModel
    {
        public event Action<BottomPanelViewModelBase> SetBottomPanelViewModelChanged;
            
        private BottomPanelViewModelBase _bottomPanelViewModel;

        public void SetBottomPanelViewModel(BottomPanelViewModelBase model)
        {
            if (_bottomPanelViewModel == model) return;
            
            _bottomPanelViewModel = model;
            
            SetBottomPanelViewModelChanged?.Invoke(_bottomPanelViewModel);
        }
    }

    public interface IMainViewModel
    {
        
    }
}