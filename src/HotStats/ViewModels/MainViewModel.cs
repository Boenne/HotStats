using System;
using System.IO;
using System.Linq;
using System.Threading;
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
        private Uri backgroundImageSource;
        private CancellationTokenSource cancellationTokenSource;

        public MainViewModel(INavigationService navigationService, IMessenger messenger,
            IDispatcherWrapper dispatcherWrapper) : base(messenger)
        {
            this.navigationService = navigationService;
            this.dispatcherWrapper = dispatcherWrapper;
            cancellationTokenSource = new CancellationTokenSource();
            BackgroundImageSource = new Uri("pack://application:,,,/Resources/defaultimage.png");
            SetBackgroundImageSource();

            messenger.Register<SettingsSavedMessage>(this, message =>
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource = new CancellationTokenSource();
                SetBackgroundImageSource();
            });
        }


        public RelayCommand LoadedCommand => new RelayCommand(() => { navigationService.NavigateTo("LoadData"); });
        public RelayCommand ClosingCommand => new RelayCommand(() => cancellationTokenSource.Cancel());

        public Uri BackgroundImageSource
        {
            get { return backgroundImageSource; }
            set { Set(() => BackgroundImageSource, ref backgroundImageSource, value); }
        }

        public void SetBackgroundImageSource()
        {
            Task.Factory.StartNew(() =>
            {
                dispatcherWrapper.BeginInvoke(async () =>
                {
                    var wallpapersPath = Settings.Default.WallpapersPath;
                    if (string.IsNullOrWhiteSpace(wallpapersPath)) return;
                    var directoryInfo = new DirectoryInfo(wallpapersPath);
                    if (!directoryInfo.Exists) return;
                    var fileInfos = directoryInfo.GetFiles();
                    if (!fileInfos.Any()) return;
                    var i = -1;
                    while (true)
                    {
                        if (i < fileInfos.Length)
                            i++;
                        if (i == fileInfos.Length)
                            i = 0;
                        BackgroundImageSource = new Uri(fileInfos[i].FullName);
                        await Task.Delay(10000);
                    }
                });
            }, cancellationTokenSource.Token);
        }
    }

    public interface IMainViewModel
    {
        RelayCommand LoadedCommand { get; }
        RelayCommand ClosingCommand { get; }
        Uri BackgroundImageSource { get; set; }
    }
}