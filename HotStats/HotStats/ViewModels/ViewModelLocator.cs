using HotStats.ViewModels.Interfaces;

namespace HotStats.ViewModels
{
    public class ViewModelLocator
    {
        public IMainViewModel MainViewModel => IoCContainer.Resolve<IMainViewModel>();
        public ILoadDataViewModel LoadDataViewModel => IoCContainer.Resolve<ILoadDataViewModel>();
        public IDataPresenterViewModel DataPresenterViewModel => IoCContainer.Resolve<IDataPresenterViewModel>();
        public IAverageStatsViewModel AverageStatsViewModel => IoCContainer.Resolve<IAverageStatsViewModel>();
    }
}