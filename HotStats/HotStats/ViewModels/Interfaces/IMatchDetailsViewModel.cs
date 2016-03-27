using System.Collections.Generic;

namespace HotStats.ViewModels.Interfaces
{
    public interface IMatchDetailsViewModel
    {
        List<PlayerViewModel> Players { get; set; }
    }
}