using HotStats.ViewModels.Interfaces;

namespace HotStats.ViewModels
{
    public class ViewModelLocator
    {
        public IMainViewModel MainViewModel => IoCContainer.Resolve<IMainViewModel>();
        public ILoadDataViewModel LoadDataViewModel => IoCContainer.Resolve<ILoadDataViewModel>();
        public IMatchesViewModel MatchesViewModel => IoCContainer.Resolve<IMatchesViewModel>();
        public IAverageStatsViewModel AverageStatsViewModel => IoCContainer.Resolve<IAverageStatsViewModel>();
        public IOpponentsAndTeammatesViewModel OpponentsAndTeammatesViewModel => IoCContainer.Resolve<IOpponentsAndTeammatesViewModel>();
        public ISelectedHeroViewModel SelectedHeroViewModel => IoCContainer.Resolve<ISelectedHeroViewModel>();
        public ITotalStatsViewModel TotalStatsViewModel => IoCContainer.Resolve<ITotalStatsViewModel>();
        public IHeroSelectorViewModel HeroSelectorViewModel => IoCContainer.Resolve<IHeroSelectorViewModel>();
        public IMatchDetailsViewModel MatchDetailsViewModel => IoCContainer.Resolve<IMatchDetailsViewModel>();
    }
}