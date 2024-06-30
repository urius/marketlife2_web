using UnityEngine;
using View.Game.Misc;
using View.Game.ShopObjects.Common;

namespace View.Game.ShopObjects.CashDesk
{
    [SelectionBase]
    public class CashDeskView : ShopObjectViewBase
    {
        private const int MaxMoneyItemsAmountX = 5;
        private const int MaxMoneyItemsAmountY = 3;
        private const int MaxMoneyItemsOnPlane = MaxMoneyItemsAmountX * MaxMoneyItemsAmountY;
        
        [SerializeField] private Transform _moneyPlaceholderInitial;
        [SerializeField] private Transform _moneyPlaceholderDeltaX;
        [SerializeField] private Transform _moneyPlaceholderDeltaY;
        [SerializeField] private Transform _moneyPlaceholderDeltaZ;
        [SerializeField] private Transform _moneyContainer;
        
        private Vector3 _moneyDeltaPositionX;
        private Vector3 _moneyDeltaPositionY;
        private Vector3 _moneyDeltaPositionZ;

        protected override void Awake()
        {
            base.Awake();

            var moneyPlaceholderInitialPosition = _moneyPlaceholderInitial.position;
            
            _moneyDeltaPositionX = _moneyPlaceholderDeltaX.position - moneyPlaceholderInitialPosition;
            _moneyDeltaPositionY = _moneyPlaceholderDeltaY.position - moneyPlaceholderInitialPosition;
            _moneyDeltaPositionZ = _moneyPlaceholderDeltaZ.position - moneyPlaceholderInitialPosition;
        }

        public void PlaceToMoneyPosition(MoneyView moneyView, int moneyPositionIndex)
        {
            moneyView.transform.SetParent(_moneyContainer);
            
            var position = GetMoneyPositionFromFlatIndex(moneyPositionIndex);
            moneyView.transform.position = GetMoneySlotWorldPosition(position);
            moneyView.transform.localRotation = Quaternion.Euler(0, 0, -45);

            moneyView.SetSortingOrder((MaxMoneyItemsAmountX - position.x + MaxMoneyItemsAmountY - position.y) * (position.z + 1));
        }

        private Vector3 GetMoneySlotWorldPosition(Vector3Int slotVectorizedIndex)
        {
            return _moneyPlaceholderInitial.position
                   + slotVectorizedIndex.x * _moneyDeltaPositionX
                   + slotVectorizedIndex.y * _moneyDeltaPositionY
                   + slotVectorizedIndex.z * _moneyDeltaPositionZ;
        }

        private static Vector3Int GetMoneyPositionFromFlatIndex(int moneyPositionIndex)
        {
            var z = moneyPositionIndex / MaxMoneyItemsOnPlane;
            var remainingIndex = moneyPositionIndex % MaxMoneyItemsOnPlane;
            var x = remainingIndex % MaxMoneyItemsAmountX;
            var y = remainingIndex / MaxMoneyItemsAmountX;
    
            return new Vector3Int(x, y, z);
        }
    }
}