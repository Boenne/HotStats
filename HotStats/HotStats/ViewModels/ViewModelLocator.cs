using HotStats.ViewModels.Interfaces;

namespace HotStats.ViewModels
{
    public class ViewModelLocator
    {
        public IMainViewModel MainViewModel => IoCContainer.Resolve<IMainViewModel>();
        public ILoadDataViewModel LoadDataViewModel => IoCContainer.Resolve<ILoadDataViewModel>();
        public IMatchesViewModel MatchesViewModel => IoCContainer.Resolve<IMatchesViewModel>();
        public IAverageStatsViewModel AverageStatsViewModel => IoCContainer.Resolve<IAverageStatsViewModel>();
        public IOpponentsViewModel OpponentsViewModel => IoCContainer.Resolve<IOpponentsViewModel>();
        public ISelectedHeroViewModel SelectedHeroViewModel => IoCContainer.Resolve<ISelectedHeroViewModel>();
        public ITotalStatsViewModel TotalStatsViewModel => IoCContainer.Resolve<ITotalStatsViewModel>();
        public IHeroSelectorViewModel HeroSelectorViewModel => IoCContainer.Resolve<IHeroSelectorViewModel>();
    }
}