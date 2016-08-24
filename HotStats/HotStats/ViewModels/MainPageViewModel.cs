using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Properties;

namespace HotStats.ViewModels
{
    public class MainPageViewModel : ObservableObject, IMainPageViewModel
    {
        private readonly IMessenger messenger;
        private bool heroSelected;
        private bool matchSelected;

        public MainPageViewModel(IMessenger messenger)
        {
            this.messenger = messenger;
            messenger.Register<HeroSelectedMessage>(this, message =>
            {
                HeroSelected = true;
            });
            messenger.Register<HeroDeselectedMessage>(this, message =>
            {
                HeroSelected = false;
            });
            messenger.Register<MatchSelectedMessage>(this, message => MatchSelected = true);
        }

        public bool HeroSelected
        {
            get { return heroSelected; }
            set
            {
                heroSelected = value;
                OnPropertyChanged();
            }
        }

        public bool MatchSelected
        {
            get { return matchSelected; }
            set
            {
                matchSelected = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand CloseMatchDetailsCommand => new RelayCommand(() => MatchSelected = false);
        public RelayCommand LoadedCommand => new RelayCommand(() =>
        {
            messenger.Send(new PlayerNameHasBeenSetMessage(Settings.Default.PlayerName));
        });
    }

    public interface IMainPageViewModel
    {
        bool HeroSelected { get; set; }
        bool MatchSelected { get; set; }
        RelayCommand CloseMatchDetailsCommand { get; }
        RelayCommand LoadedCommand { get; }
    }
}