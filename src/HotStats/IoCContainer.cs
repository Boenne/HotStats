using LightInject;

namespace HotStats
{
    public static class IoCContainer
    {
        private static readonly ServiceContainer ServiceContainer = new ServiceContainer();

        public static void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            ServiceContainer.Register<TInterface, TImplementation>();
        }

        public static void Register<TInterface>(TInterface instance)
        {
            ServiceContainer.RegisterInstance(instance);
        }

        public static void RegisterSingleton<TInterface, TImplementation>() where TImplementation : TInterface
        {
            ServiceContainer.Register<TInterface, TImplementation>(new PerContainerLifetime());
        }

        public static TInterface Resolve<TInterface>()
        {
            return ServiceContainer.GetInstance<TInterface>();
        }

        public static void ResolveAll()
        {
            foreach (var service in ServiceContainer.AvailableServices)
            {
                ServiceContainer.GetInstance(service.ServiceType);
            }
        }
    }
}