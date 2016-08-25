using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Properties;

namespace HotStats.ViewModels
{
    public class MainPageViewModel : ViewModelBase, IMainPageViewModel
    {
        private readonly IMessenger messenger;
        private bool matchSelected;
        private string rightImageSource;

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

        public string RightImageSource
        {
            get { return Environment.CurrentDirectory + "/rightpic.jpg"; }
        }

        public RelayCommand CloseMatchDetailsCommand => new RelayCommand(() => MatchSelected = false);

        public RelayCommand LoadedCommand
            => new RelayCommand(() => { messenger.Send(new PlayerNameHasBeenSetMessage(Settings.Default.PlayerName)); });
    }

    public interface IMainPageViewModel
    {
        bool MatchSelected { get; set; }
        string RightImageSource { get; }
        RelayCommand CloseMatchDetailsCommand { get; }
        RelayCommand LoadedCommand { get; }
    }
}