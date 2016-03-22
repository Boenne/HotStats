using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace HotStats.ViewModels.Interfaces
{
    public interface IDataPresenterViewModel
    {
        string PlayerName { get; set; }
        bool PlayerNameIsSet { get; set; }
        bool PresentingData { get; set; }
        IEnumerable<IGrouping<string, MatchViewModel>> Matches { get; set; }
        ICommand LoadedCommand { get; }
        ICommand SetPlayerNameCommand { get; }
        ICommand HeroSelectedCommand { get; }
    }
}