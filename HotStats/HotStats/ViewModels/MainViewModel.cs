using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.ViewModels.Interfaces;

namespace HotStats.ViewModels
{
    public class MainViewModel : ObservableObject, IMainViewModel
    {
        private bool dataHasBeenLoaded;
        private bool heroSelected;

        public MainViewModel(IMessenger messenger)
        {
            messenger.Register<DataHasBeenLoadedMessage>(this, message => DataHasBeenLoaded = true);
            messenger.Register<HeroSelectedMessage>(this, message =>
            {
                HeroSelected = true;
                DataHasBeenLoaded = false;
            });
            messenger.Register<HeroDeselectedMessage>(this, message =>
            {
                HeroSelected = false;
                DataHasBeenLoaded = true;
            });
        }

        public bool DataHasBeenLoaded
        {
            get { return dataHasBeenLoaded; }
            set
            {
                dataHasBeenLoaded = value;
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
    }
}