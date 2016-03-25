using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.ViewModels.Interfaces;

namespace HotStats.ViewModels
{
    public class MainViewModel : ObservableObject, IMainViewModel
    {
        private bool dataHasBeenLoaded;

        public MainViewModel(IMessenger messenger)
        {
            messenger.Register<DataHasBeenLoadedMessage>(this, message => DataHasBeenLoaded = true);
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
    }
}