using UnityEngine;

namespace View.Game.ShopObjects
{
    public abstract class ShopObjectMediatorBase<TModel> : MediatorWithModelBase<TModel>
        where TModel : class
    {
        public override void Mediate(Transform transform)
        {
            base.Mediate(transform);

            UpdateSorting();
        }

        protected abstract void UpdateSorting();
    }
}