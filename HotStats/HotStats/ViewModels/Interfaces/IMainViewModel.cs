using System.Windows.Input;

namespace HotStats.ViewModels.Interfaces
{
    public interface IMainViewModel
    {
        bool PlayerNameSet { get; set; }
        bool HeroSelected { get; set; }
        bool MatchSelected { get; set; }
        ICommand CloseMatchDetailsCommand { get; }
    }
}