﻿using System.Collections.Generic;

namespace HotStats.ViewModels.Interfaces
{
    public interface IOpponentsViewModel
    {
        List<OpponentViewModel> Opponents { get; set; }
        List<OpponentViewModel> Teammates { get; set; }
        bool PlayerNameIsSet { get; set; }
    }
}