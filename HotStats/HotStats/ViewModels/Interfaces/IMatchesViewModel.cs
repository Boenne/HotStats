using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;

namespace HotStats.ViewModels.Interfaces
{
    public interface IMatchesViewModel
    {
        List<MatchViewModel> Matches { get; set; }
        RelayCommand<MatchViewModel> SelectMatchCommand { get; }
    }
}