using GalaSoft.MvvmLight.Command;

namespace HotStats.ViewModels.Interfaces
{
    public interface ISelectedHeroViewModel
    {
        bool HeroSelected { get; set; }
        string Hero { get; set; }
        RelayCommand DeselectHeroCommand { get; }
    }
}