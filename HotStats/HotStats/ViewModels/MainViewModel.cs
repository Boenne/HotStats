using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Wrappers;

namespace HotStats.ViewModels
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private readonly INavigationService navigationService;
        private readonly IDispatcherWrapper dispatcherWrapper;
        private Uri backgroundImageSource;
        private bool running = true;
        private CancellationTokenSource cancellationTokenSource;

        public MainViewModel(INavigationService navigationService, IMessenger messenger, IDispatcherWrapper dispatcherWrapper)
        {
            this.navigationService = navigationService;
            this.dispatcherWrapper = dispatcherWrapper;
            cancellationTokenSource = new CancellationTokenSource();
            BackgroundImageSource = new Uri("pack://application:,,,/Resources/defaultimage.png");
            SetBackgroundImageSource(cancellationTokenSource);
            
            messenger.Register<SettingsSavedMessage>(this, message =>
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource = new CancellationTokenSource();
                SetBackgroundImageSource(cancellationTokenSource);
            });
        }


        public RelayCommand LoadedCommand => new RelayCommand(() => { navigationService.NavigateTo("LoadData"); });
        public RelayCommand ClosingCommand => new RelayCommand(() => cancellationTokenSource.Cancel());

        public Uri BackgroundImageSource
        {
            get { return backgroundImageSource; }
            set { Set(() => BackgroundImageSource, ref backgroundImageSource, value); }
        }

        public void SetBackgroundImageSource(CancellationTokenSource cancellationTokenSource)
        {
            Task.Factory.StartNew(() =>
            {
                dispatcherWrapper.BeginInvoke(async () =>
                {
                    var path = Environment.CurrentDirectory + "/pics";
                    var directoryInfo = new DirectoryInfo(path);
                    if (!directoryInfo.Exists) return;
                    var fileInfos = directoryInfo.GetFiles();
                    if (!fileInfos.Any()) return;
                    var i = -1;
                    while (running)
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