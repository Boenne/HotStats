using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Navigation;
using HotStats.Properties;
using HotStats.Wrappers;

namespace HotStats.ViewModels
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly IMessenger messenger;
        private readonly INavigationService navigationService;
        private string backgroundImageSource;
        private bool endTask;

        public MainViewModel(INavigationService navigationService, IMessenger messenger,
            IDispatcherWrapper dispatcherWrapper) : base(messenger)
        {
            this.navigationService = navigationService;
            this.messenger = messenger;
            this.dispatcherWrapper = dispatcherWrapper;
            SetBackgroundImageSource();

            messenger.Register<SettingsSavedMessage>(this, async message =>
            {
                endTask = true;
                await Task.Delay(500);
                SetBackgroundImageSource();
            });
        }

        public RelayCommand LoadedCommand
            => new RelayCommand(() =>
            {
                if (Settings.Default.UpgradeRequired)
                {
                    Settings.Default.Upgrade();
                    Settings.Default.UpgradeRequired = false;
                    Settings.Default.Save();
                }
                navigationService.NavigateTo(string.IsNullOrEmpty(Settings.Default.PlayerName)
                    ? NavigationFrames.SetPlayerName
                    : NavigationFrames.LoadData);
            });

        public RelayCommand ClosingCommand => new RelayCommand(() =>
        {
            endTask = true;
            WebClients.WebClient.Dispose();
            WebClients.HttpClient.Dispose();
        });

        public RelayCommand<KeyEventArgs> KeyUpCommand => new RelayCommand<KeyEventArgs>(KeyUp);

        public string BackgroundImageSource
        {
            get { return backgroundImageSource; }
            set { Set(() => BackgroundImageSource, ref backgroundImageSource, value); }
        }

        public void SetBackgroundImageSource()
        {
            endTask = false;
            Task.Run(() =>
            {
                dispatcherWrapper.BeginInvoke(async () =>
                {
                    var wallpapersPath = Settings.Default.WallpapersPath;
                    if (string.IsNullOrWhiteSpace(wallpapersPath))
                    {
                        BackgroundImageSource = null;
                        return;
                    }
                    if (File.Exists(Settings.Default.WallpapersPath))
                    {
                        BackgroundImageSource = Settings.Default.WallpapersPath;
                        return;
                    }
                    var directoryInfo = new DirectoryInfo(wallpapersPath);
                    if (!directoryInfo.Exists) return;
                    var fileInfos =
                        directoryInfo.GetFiles()
                            .Where(x => x.Name.EndsWith(".jpg") || x.Name.EndsWith(".jpeg") || x.Name.EndsWith(".png"))
                            .ToList();
                    if (!fileInfos.Any()) return;
                    var i = -1;
                    while (true)
                    {
                        if (endTask) break;
                        if (i < fileInfos.Count)
                            i++;
                        if (i == fileInfos.Count)
                            i = 0;
                        BackgroundImageSource = fileInfos[i].FullName;
                        await WaitSeconds(10);
                    }
                });
            });
        }

        public void KeyUp(KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key != Key.Escape) return;
            messenger.Send(new CloseMatchDetailsMessage());
        }

        private async Task WaitSeconds(int seconds)
        {
            double secondsCount = 0;
            while (secondsCount < seconds)
            {
                if (endTask)
                    break;
                secondsCount += 0.5;
                await Task.Delay(500);
            }
        }
    }

    public interface IMainViewModel
    {
        RelayCommand LoadedCommand { get; }
        RelayCommand ClosingCommand { get; }
        RelayCommand<KeyEventArgs> KeyUpCommand { get; }
        string BackgroundImageSource { get; set; }
    }
}