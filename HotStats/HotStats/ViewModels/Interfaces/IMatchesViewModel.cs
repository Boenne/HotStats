using System.Collections.Generic;
using System.Windows.Input;

namespace HotStats.ViewModels.Interfaces
{
    public interface IMatchesViewModel
    {
        bool HeroSelected { get; set; }
        List<MatchViewModel> Matches { get; set; }
        ICommand SelectMatchCommand { get; }
    }
}