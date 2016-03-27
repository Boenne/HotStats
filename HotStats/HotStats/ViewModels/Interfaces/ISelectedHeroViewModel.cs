using System.Windows.Input;

namespace HotStats.ViewModels.Interfaces
{
    public interface ISelectedHeroViewModel
    {
        string Hero { get; set; }
        ITotalStatsViewModel TotalStatsViewModel { get; set; }
        ICommand DeselectHeroCommand { get; }
    }
}