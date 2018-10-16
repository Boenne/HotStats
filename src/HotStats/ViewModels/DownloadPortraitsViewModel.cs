using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Navigation;
using HotStats.Services;
using HotStats.Wrappers;

namespace HotStats.ViewModels
{
    public interface IDownloadPortraitsViewModel
    {
        string Text { get; set; }
        RelayCommand LoadedCommand { get; }
    }

    public class DownloadPortraitsViewModel : ViewModelBase, IDownloadPortraitsViewModel
    {
        private readonly IMessageBoxWrapper messageBoxWrapper;
        private readonly INavigationService navigationService;
        private readonly IPortraitDownloader portraitDownloader;
        private string text;

        public DownloadPortraitsViewModel(IMessenger messenger, IPortraitDownloader portraitDownloader,
            INavigationService navigationService, IMessageBoxWrapper messageBoxWrapper) :
            base(messenger)
        {
            this.portraitDownloader = portraitDownloader;
            this.navigationService = navigationService;
            this.messageBoxWrapper = messageBoxWrapper;
        }

        public string Text
        {
            get => text;
            set { Set(() => Text, ref text, value); }
        }

        public RelayCommand LoadedCommand => new RelayCommand(async () => await DownloadPortraits());

        public async Task DownloadPortraits()
        {
            try
            {
                Text = "Scanning for new portraits...";
                var portraits = await portraitDownloader.GetPortraits();

                Text = "Downloading portraits...";
                await portraitDownloader.DownloadPortraits(portraits, false);
            }
            catch
            {
                messageBoxWrapper.Show("Error downloading portraits");
            }
            navigationService.NavigateTo(NavigationFrames.MainPage);
        }
    }
}