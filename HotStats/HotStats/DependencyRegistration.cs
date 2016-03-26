﻿using HotStats.Messaging;
using HotStats.Services;
using HotStats.Services.Interfaces;
using HotStats.ViewModels;
using HotStats.ViewModels.Interfaces;
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

            IoCContainer.Register<IMainViewModel, MainViewModel>();
            IoCContainer.Register<ILoadDataViewModel, LoadDataViewModel>();
            IoCContainer.Register<IMatchesViewModel, MatchesViewModel>();
            IoCContainer.Register<IAverageStatsViewModel, AverageStatsViewModel>();
            IoCContainer.Register<ITotalStatsViewModel, TotalStatsViewModel>();
            IoCContainer.Register<ISelectedHeroViewModel, SelectedHeroViewModel>();
            IoCContainer.Register<IOpponentsAndTeammatesViewModel, OpponentsAndTeammatesViewModel>();
            IoCContainer.Register<IHeroSelectorViewModel, HeroSelectorViewModel>();
            IoCContainer.Register<IMatchDetailsViewModel, MatchDetailsViewModel>();
        }
    }
}