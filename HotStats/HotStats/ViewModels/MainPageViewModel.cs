using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Properties;
using HotStats.Windows;

namespace HotStats.ViewModels
{
    public class MainPageViewModel : ViewModelBase, IMainPageViewModel
    {
        private readonly IMessenger messenger;
        private bool matchSelected;

        public MainPageViewModel(IMessenger messenger)
        {
            this.messenger = messenger;
            messenger.Register<MatchSelectedMessage>(this, message => MatchSelected = true);
        }

        public bool MatchSelected
        {
            get { return matchSelected; }
            set { Set(() => MatchSelected, ref matchSelected, value); }
        }

        public RelayCommand CloseMatchDetailsCommand => new RelayCommand(() => MatchSelected = false);

        public RelayCommand LoadedCommand
            => new RelayCommand(() => { messenger.Send(new PlayerNameHasBeenSetMessage(Settings.Default.PlayerName)); });

        public RelayCommand OpenSettingsCommand => new RelayCommand(() => new SettingsWindow().Show());
    }

    public interface IMainPageViewModel
    {
        bool MatchSelected { get; set; }
        RelayCommand CloseMatchDetailsCommand { get; }
        RelayCommand LoadedCommand { get; }
        RelayCommand OpenSettingsCommand { get; }
    }
}