using Data;
using Infra.Instance;
using Model.BuildPoint;
using Utils;

namespace View.Game.BuildPoint
{
    public class BuildPointMediator : MediatorWithModelBase<BuildPointModel>
    {
        private readonly IGridCalculator _gridCalculator = Instance.Get<IGridCalculator>();
        
        private BuildPointView _view;

        protected override void MediateInternal()
        {
            _view = InstantiatePrefab<BuildPointView>(PrefabKey.BuildPoint);
            
            _view.transform.position = _gridCalculator.GetCellCenterWorld(TargetModel.CellCoords);
            UpdateMoneyText();

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            Destroy(_view);
            _view = null;
        }

        private void Subscribe()
        {
            TargetModel.MoneyToBuildLeftChanged += OnMoneyToBuildLeftChanged;
        }

        private void Unsubscribe()
        {
            TargetModel.MoneyToBuildLeftChanged -= OnMoneyToBuildLeftChanged;
        }

        private void OnMoneyToBuildLeftChanged()
        {
            UpdateMoneyText();
        }

        private void UpdateMoneyText()
        {
            _view.SetText(TargetModel.MoneyToBuildLeft.ToString());
        }
    }
}