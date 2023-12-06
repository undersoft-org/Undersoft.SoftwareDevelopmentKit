namespace Undersoft.SDK.Service
{
    public class ServiceRegistryObject<T> : ServiceRegistryObject, IServiceRegistryObject<T>
    {
        public new T Value { get; set; }

        public ServiceRegistryObject()
        {
        }

        public ServiceRegistryObject(T obj)
        {
            Value = obj;
        }
    }

    public class ServiceRegistryObject : IServiceRegistryObject
    {
        public object Value { get; set; }

        public ServiceRegistryObject()
        {
        }

        public ServiceRegistryObject(object obj)
        {
            Value = obj;
        }
    }
}