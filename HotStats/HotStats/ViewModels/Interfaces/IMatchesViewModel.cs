using System.Collections.Generic;

namespace HotStats.ViewModels.Interfaces
{
    public interface IMatchesViewModel
    {
        bool HeroSelected { get; set; }
        List<MatchViewModel> Matches { get; set; }
    }
}