using UnityEngine;
using View.Game.Misc;

namespace View.Game.Truck
{
    public class TruckView : MonoBehaviour
    {
        [SerializeField] private ProductsBoxView[] _productsBoxViews;
        [SerializeField] private Transform _truckBoxCapTransform;

        public ProductsBoxView GetProductBoxView(int index)
        {
            return _productsBoxViews[index];
        }
    }
}