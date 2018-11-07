namespace HotStats.ViewModels
{
    public class ViewModelLocator
    {
        public IMainViewModel MainViewModel => IoCContainer.Resolve<IMainViewModel>();
        public IMainPageViewModel MainPageViewModel => IoCContainer.Resolve<IMainPageViewModel>();
        public ILoadDataViewModel LoadDataViewModel => IoCContainer.Resolve<ILoadDataViewModel>();
        public ISetPlayerNameViewModel SetPlayerNameViewModel => IoCContainer.Resolve<ISetPlayerNameViewModel>();
        public IMatchesViewModel MatchesViewModel => IoCContainer.Resolve<IMatchesViewModel>();
        public IAverageStatsViewModel AverageStatsViewModel => IoCContainer.Resolve<IAverageStatsViewModel>();
        public IOpponentsAndTeammatesViewModel OpponentsAndTeammatesViewModel => IoCContainer.Resolve<IOpponentsAndTeammatesViewModel>();
        public ISelectedHeroViewModel SelectedHeroViewModel => IoCContainer.Resolve<ISelectedHeroViewModel>();
        public ITotalStatsViewModel TotalStatsViewModel => IoCContainer.Resolve<ITotalStatsViewModel>();
        public IHeroSelectorViewModel HeroSelectorViewModel => IoCContainer.Resolve<IHeroSelectorViewModel>();
        public IMatchDetailsViewModel MatchDetailsViewModel => IoCContainer.Resolve<IMatchDetailsViewModel>();
        public ISettingsViewModel SettingsViewModel => IoCContainer.Resolve<ISettingsViewModel>();
        public IDownloadPortraitsViewModel DownloadPortraitsViewModel => IoCContainer.Resolve<IDownloadPortraitsViewModel>();
        public ICheckVersionViewModel CheckVersionViewModel => IoCContainer.Resolve<ICheckVersionViewModel>();
        public IFiltersViewModel FiltersViewModel => IoCContainer.Resolve<IFiltersViewModel>();
    }
}