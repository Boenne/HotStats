using GalaSoft.MvvmLight.Command;

namespace HotStats.ViewModels.Interfaces
{
    public interface IMainViewModel
    {
        RelayCommand LoadedCommand { get; }
    }
}