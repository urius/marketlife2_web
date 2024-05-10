using Events;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using UnityEngine;

namespace Systems
{
    public class PlayerCharSystem : ISystem
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private ShopModel _shopModel;
        private PlayerModel _playerModel;
        private PlayerCharModel _playerCharModel;

        public void Start()
        {
            _playerModel = _playerModelHolder.PlayerModel;
            _shopModel = _playerModel.ShopModel;
            _playerCharModel = _playerModel.PlayerCharModel;
            
            Subscribe();
        }

        public void Stop()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<RequestPlayerCellChangeEvent>(OnRequestPlayerCellChangeEvent);
            _playerCharModel.CellPositionChanged += OnCellPositionChanged;
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<RequestPlayerCellChangeEvent>(OnRequestPlayerCellChangeEvent);
            _playerCharModel.CellPositionChanged -= OnCellPositionChanged;
        }

        private void OnRequestPlayerCellChangeEvent(RequestPlayerCellChangeEvent e)
        {
            _playerCharModel.SetCellPosition(e.CellCoords);
        }

        private void OnCellPositionChanged(Vector2Int cellPosition)
        {
            if (_shopModel.BuildPoints.TryGetValue(cellPosition, out var buildPoint)
                && _playerModel.Money > 0)
            {
                //trigger start spend animation
            }
        }
    }
}