using System.Collections.Generic;

namespace HotStats.ViewModels.Interfaces
{
    public interface IOpponentsAndTeammatesViewModel
    {
        List<OpponentViewModel> Opponents { get; set; }
        List<OpponentViewModel> Teammates { get; set; }
    }
}