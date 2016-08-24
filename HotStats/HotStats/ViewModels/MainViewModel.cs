using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using HotStats.ViewModels.Interfaces;

namespace HotStats.ViewModels
{
    public class MainViewModel : ObservableObject, IMainViewModel
    {
        private readonly INavigationService navigationService;

        public MainViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        public RelayCommand LoadedCommand => new RelayCommand(() => { navigationService.NavigateTo("LoadData"); });
    }
}