using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace HotStats.ViewModels.Interfaces
{
    public interface IHeroSelectorViewModel
    {
        List<string> Heroes { get; set; }
        bool PlayerNameIsSet { get; set; }
        bool ShowHeroLeague { get; set; }
        bool ShowQuickMatches { get; set; }
        bool ShowUnranked { get; set; }
        DateTime DateFilter { get; set; }
        DateTime EarliestDate { get; set; }
        DateTime TodaysDate { get; set; }
        ICommand SelectHeroCommand { get; }
        ICommand RemoveDateFilterCommand { get; }
        ICommand ReloadDataCommand { get; }
    }
}