using HotStats.Messaging;
using HotStats.Navigation;
using HotStats.Services;
using HotStats.Services.Interfaces;
using HotStats.ViewModels;
using HotStats.Wrappers;

namespace HotStats
{
    public class DependencyRegistration
    {
        public static void RegisterDependencies()
        {
            IoCContainer.Register<IDispatcherWrapper, DispatcherWrapper>();
            IoCContainer.Register<IMessageBoxWrapper, MessageBoxWrapper>();
            IoCContainer.Register<IMessenger, Messenger>();

            IoCContainer.Register<IParser, Parser>();
            IoCContainer.RegisterSingleton<IReplayRepository, ReplayRepository>();
            IoCContainer.RegisterSingleton<IHeroDataRepository, HeroDataRepository>();
            IoCContainer.Register<IDataLoader, DataLoader>();
            IoCContainer.Register<IPortraitDownloader, PortraitDownloader>();
            IoCContainer.Register<IHeroDataDownloader, HeroDataDownloader>();
            IoCContainer.Register<IVersionChecker, VersionChecker>();
            IoCContainer.Register<IInternetConnectivityService, InternetConnectivityService>();

            IoCContainer.Register<IMainViewModel, MainViewModel>();
            IoCContainer.Register<IMainPageViewModel, MainPageViewModel>();
            IoCContainer.Register<ILoadDataViewModel, LoadDataViewModel>();
            IoCContainer.Register<ISetPlayerNameViewModel, SetPlayerNameViewModel>();
            IoCContainer.Register<IMatchesViewModel, MatchesViewModel>();
            IoCContainer.Register<IAverageStatsViewModel, AverageStatsViewModel>();
            IoCContainer.Register<ITotalStatsViewModel, TotalStatsViewModel>();
            IoCContainer.Register<ISelectedHeroViewModel, SelectedHeroViewModel>();
            IoCContainer.Register<IOpponentsAndTeammatesViewModel, OpponentsAndTeammatesViewModel>();
            IoCContainer.Register<IHeroSelectorViewModel, HeroSelectorViewModel>();
            IoCContainer.Register<ISettingsViewModel, SettingsViewModel>();
            IoCContainer.Register<IMatchDetailsViewModel, MatchDetailsViewModel>();
            IoCContainer.Register<IDownloadPortraitsViewModel, DownloadPortraitsViewModel>();
            IoCContainer.Register<ICheckVersionViewModel, CheckVersionViewModel>();

            var navigationService = new NavigationService();
            navigationService.AddPage(NavigationFrames.LoadData);
            navigationService.AddPage(NavigationFrames.SetPlayerName);
            navigationService.AddPage(NavigationFrames.MainPage);
            navigationService.AddPage(NavigationFrames.DownloadPortraits);
            navigationService.AddPage(NavigationFrames.CheckVersion);

            IoCContainer.Register<INavigationService>(navigationService);
        }
    }
}