using System.Collections.Generic;

namespace HotStats.ViewModels.Interfaces
{
    public interface IOpponentsViewModel
    {
        List<OpponentViewModel> Opponents { get; set; }
        bool HeroSelected { get; set; }
    }
}