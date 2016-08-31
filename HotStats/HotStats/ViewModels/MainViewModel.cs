using System;
using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Properties;

namespace HotStats.ViewModels
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private readonly INavigationService navigationService;
        private Uri backgroundImageSource;

        public MainViewModel(INavigationService navigationService, IMessenger messenger)
        {
            this.navigationService = navigationService;
            SetBackgroundImageSource();
            messenger.Register<SettingsSavedMessage>(this, message => SetBackgroundImageSource());
        }


        public RelayCommand LoadedCommand => new RelayCommand(() => { navigationService.NavigateTo("LoadData"); });

        public Uri BackgroundImageSource
        {
            get { return backgroundImageSource; }
            set { Set(() => BackgroundImageSource, ref backgroundImageSource, value); }
        }

        public void SetBackgroundImageSource()
        {
            var file = Settings.Default.BackgroundPath;
            BackgroundImageSource = File.Exists(file)
                ? new Uri(file)
                : new Uri("pack://application:,,,/Resources/defaultimage.png");
        }
    }

    public interface IMainViewModel
    {
        RelayCommand LoadedCommand { get; }
        Uri BackgroundImageSource { get; set; }
    }
}