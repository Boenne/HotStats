using System.Collections.Generic;
using System.Windows.Input;

namespace HotStats.ViewModels.Interfaces
{
    public interface IHeroSelectorViewModel
    {
        List<string> Heroes { get; set; }
        bool PlayerNameIsSet { get; set; }
        ICommand SelectHeroCommand { get; }
    }
}