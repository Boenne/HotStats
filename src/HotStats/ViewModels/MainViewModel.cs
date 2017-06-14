using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Properties;
using HotStats.Wrappers;

namespace HotStats.ViewModels
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly INavigationService navigationService;
        private string backgroundImageSource;
        private bool endTask;

        public MainViewModel(INavigationService navigationService, IMessenger messenger,
            IDispatcherWrapper dispatcherWrapper) : base(messenger)
        {
            this.navigationService = navigationService;
            this.dispatcherWrapper = dispatcherWrapper;
            SetBackgroundImageSource();

            messenger.Register<SettingsSavedMessage>(this, async message =>
            {
                endTask = true;
                await Task.Delay(500);
                SetBackgroundImageSource();
            });
        }


        public RelayCommand LoadedCommand => new RelayCommand(() => { navigationService.NavigateTo("LoadData"); });
        public RelayCommand ClosingCommand => new RelayCommand(() => endTask = true);

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
        string BackgroundImageSource { get; set; }
    }
}