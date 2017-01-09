using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Windows;

namespace HotStats.ViewModels
{
    public class MainPageViewModel : ViewModelBase, IMainPageViewModel
    {
        private bool matchSelected;

        public MainPageViewModel(IMessenger messenger) : base(messenger)
        {
            messenger.Register<MatchSelectedMessage>(this, message => MatchSelected = true);
        }

        public bool MatchSelected
        {
            get { return matchSelected; }
            set { Set(() => MatchSelected, ref matchSelected, value); }
        }

        public RelayCommand CloseMatchDetailsCommand => new RelayCommand(() => MatchSelected = false);

        public RelayCommand OpenSettingsCommand => new RelayCommand(() => new SettingsWindow().Show());
    }

    public interface IMainPageViewModel
    {
        bool MatchSelected { get; set; }
        RelayCommand CloseMatchDetailsCommand { get; }
        RelayCommand OpenSettingsCommand { get; }
    }
}