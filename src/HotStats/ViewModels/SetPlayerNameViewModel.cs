using GalaSoft.MvvmLight.CommandWpf;
using HotStats.Messaging;
using HotStats.Navigation;
using HotStats.Properties;

namespace HotStats.ViewModels
{
    public class SetPlayerNameViewModel : ViewModelBase, ISetPlayerNameViewModel
    {
        private readonly INavigationService navigationService;
        private string playerName;

        public SetPlayerNameViewModel(INavigationService navigationService, IMessenger messenger) : base(messenger)
        {
            this.navigationService = navigationService;
        }

        public string PlayerName
        {
            get { return playerName; }
            set { Set(() => PlayerName, ref playerName, value); }
        }

        public RelayCommand SetPlayerNameCommand => new RelayCommand(SetPlayerName);

        public RelayCommand LoadedCommand => new RelayCommand(StartUp);

        public void SetPlayerName()
        {
            if (string.IsNullOrEmpty(PlayerName)) return;
            Settings.Default.PlayerName = PlayerName.ToLower();
            Settings.Default.Save();
            navigationService.NavigateTo(NavigationFrames.MainPage);
        }

        public void StartUp()
        {
            PlayerName = Settings.Default.PlayerName;
        }
    }

    public interface ISetPlayerNameViewModel
    {
        string PlayerName { get; set; }
        RelayCommand SetPlayerNameCommand { get; }
        RelayCommand LoadedCommand { get; }
    }
}