using UnityEngine;

namespace View
{
    public abstract class MediatorWithModelBase<TModel> : MediatorBase
        where TModel : class
    {
        protected TModel TargetModel { get; private set; }

        public void Mediate(Transform transform, TModel model)
        {
            TargetModel = model;

            Mediate(transform);
        }

        public override void Unmediate()
        {
            base.Unmediate();

            TargetModel = null;
        }
    }
}