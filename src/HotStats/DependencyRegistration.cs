using System;
using GalaSoft.MvvmLight.Views;
using HotStats.Messaging;
using HotStats.Services;
using HotStats.Services.Interfaces;
using HotStats.ViewModels;
using HotStats.Wrappers;
using OpponentsAndTeammatesViewModel = HotStats.ViewModels.OpponentsAndTeammatesViewModel;

namespace HotStats
{
    public class DependencyRegistration
    {
        public static void RegisterDependencies()
        {
            IoCContainer.Register<IDispatcherWrapper, DispatcherWrapper>();
            IoCContainer.Register<IMessenger, Messenger>();

            IoCContainer.Register<IParser, Parser>();
            IoCContainer.RegisterSingleton<IReplayRepository, ReplayRepository>();
            IoCContainer.RegisterSingleton<IDataLoader, DataLoader>();

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

            var navigationService = new NavigationService();
            navigationService.AddPage("LoadData", new Uri("../UserControls/LoadDataUserControl.xaml", UriKind.Relative));
            navigationService.AddPage("SetPlayerName",
                new Uri("../UserControls/SetPlayerNameUserControl.xaml", UriKind.Relative));
            navigationService.AddPage("MainPage", new Uri("../UserControls/MainPageUserControl.xaml", UriKind.Relative));

            IoCContainer.Register<INavigationService>(navigationService);
        }
    }
}