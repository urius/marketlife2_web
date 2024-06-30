namespace View.Game.Shared
{
    public interface ISortableView
    {
        public void SetSortingOrder(int order);
        void SetSortingLayerName(string sortingLayerName);
    }
}