using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace HotStats.ViewModels.Interfaces
{
    public interface IDataPresenterViewModel
    {
        bool PresentingData { get; set; }
        IEnumerable<IGrouping<string, MatchViewModel>> Matches { get; set; }
        ICommand HeroSelectedCommand { get; }
    }
}