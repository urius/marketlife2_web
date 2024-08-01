using Model.People;

namespace Events
{
    public struct PutProductOnShelfHalfAnimationEvent
    {
        public readonly ProductBoxModel ProductBoxModel;

        public PutProductOnShelfHalfAnimationEvent(ProductBoxModel productBoxModel)
        {
            ProductBoxModel = productBoxModel;
        }
    }
}