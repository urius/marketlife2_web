namespace Infra.Instance
{

    public static class Instance
    {
        public static T Get<T>() where T : class
        {
            return InstanceHolder<T>.Get();
        }
    }
}