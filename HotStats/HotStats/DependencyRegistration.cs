using HotStats.Messaging;
using HotStats.ViewModels;
using HotStats.ViewModels.Interfaces;
using HotStats.Wrappers;

namespace HotStats
{
    public class DependencyRegistration
    {
        public static void RegisterDependencies()
        {
            IoCContainer.Register<IDispatcherWrapper, DispatcherWrapper>();
            IoCContainer.Register<IMessenger, Messenger>();

            IoCContainer.Register<IParser, Parser>();

            IoCContainer.Register<IMainViewModel, MainViewModel>();
            IoCContainer.Register<ILoadDataViewModel, LoadDataViewModel>();
            IoCContainer.Register<IDataPresenterViewModel, DataPresenterViewModel>();
        }
    }
}