using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;

namespace HotStats.ViewModels.Interfaces
{
    public interface IHeroSelectorViewModel
    {
        List<string> Heroes { get; set; }
        bool ShowHeroLeague { get; set; }
        bool ShowQuickMatches { get; set; }
        bool ShowUnranked { get; set; }
        DateTime DateFilter { get; set; }
        DateTime EarliestDate { get; set; }
        DateTime TodaysDate { get; set; }
        RelayCommand<string> SelectHeroCommand { get; }
        RelayCommand RemoveDateFilterCommand { get; }
        RelayCommand ReloadDataCommand { get; }
    }
}