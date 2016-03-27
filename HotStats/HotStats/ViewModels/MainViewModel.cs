using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.ViewModels.Interfaces;

namespace HotStats.ViewModels
{
    public class MainViewModel : ObservableObject, IMainViewModel
    {
        private bool playerNameSet;
        private bool heroSelected;
        private bool matchSelected;

        public MainViewModel(IMessenger messenger)
        {
            messenger.Register<PlayerNameHasBeenSetMessage>(this, message => PlayerNameSet = true);
            messenger.Register<HeroSelectedMessage>(this, message =>
            {
                HeroSelected = true;
                PlayerNameSet = false;
            });
            messenger.Register<HeroDeselectedMessage>(this, message =>
            {
                HeroSelected = false;
                PlayerNameSet = true;
            });
            messenger.Register<MatchSelectedMessage>(this, message => MatchSelected = true);
        }

        public bool PlayerNameSet
        {
            get { return playerNameSet; }
            set
            {
                playerNameSet = value;
                OnPropertyChanged();
            }
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

        public ICommand CloseMatchDetailsCommand => new RelayCommand(() => MatchSelected = false);
    }
}