using System;
using System.IO;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Properties;
using HotStats.Services;
using HotStats.Windows;
using HotStats.Wrappers;

namespace HotStats.ViewModels
{
    public interface ISettingsViewModel
    {
        RelayCommand<IClosable> LoadedCommand { get; }
        RelayCommand SaveSettingsCommand { get; }
        RelayCommand DownloadCommand { get; }
        string WallpapersPath { get; set; }
        string BackgroundColorSetting { get; set; }
        string TextColorSetting { get; set; }
        string BorderColorSetting { get; set; }
        bool EnableWallpaper { get; set; }
        bool UseMasterPortraits { get; set; }
        bool Downloading { get; set; }
    }

    public class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly IPortraitDownloader portraitDownloader;
        private readonly IMessageBoxWrapper messageBoxWrapper;
        private readonly IMessenger messenger;
        private string backgroundColorSetting;
        private string borderColorSetting;
        private bool downloading;
        private bool enableWallpaper;
        private string textColorSetting;
        private bool useMasterPortraits = Settings.Default.UseMasterPortraits;
        private string wallpapersPath;
        private IClosable window;

        public SettingsViewModel(IMessenger messenger, IPortraitDownloader portraitDownloader, IMessageBoxWrapper messageBoxWrapper) : base(messenger)
        {
            this.messenger = messenger;
            this.portraitDownloader = portraitDownloader;
            this.messageBoxWrapper = messageBoxWrapper;
        }

        public RelayCommand<IClosable> LoadedCommand => new RelayCommand<IClosable>(window =>
        {
            this.window = window;

            WallpapersPath = Settings.Default.WallpapersPath;
            BackgroundColorSetting = Settings.Default.BackgroundColor;
            TextColorSetting = Settings.Default.TextColor;
            BorderColorSetting = Settings.Default.BorderColor;

            EnableWallpaper = !string.IsNullOrEmpty(Settings.Default.WallpapersPath);
        });

        public RelayCommand SaveSettingsCommand => new RelayCommand(SaveSettings);

        public string WallpapersPath
        {
            get { return wallpapersPath; }
            set { Set(() => WallpapersPath, ref wallpapersPath, value); }
        }

        public string BackgroundColorSetting
        {
            get { return backgroundColorSetting; }
            set { Set(() => BackgroundColorSetting, ref backgroundColorSetting, value); }
        }

        public string TextColorSetting
        {
            get { return textColorSetting; }
            set { Set(() => TextColorSetting, ref textColorSetting, value); }
        }

        public string BorderColorSetting
        {
            get { return borderColorSetting; }
            set { Set(() => BorderColorSetting, ref borderColorSetting, value); }
        }

        public bool EnableWallpaper
        {
            get { return enableWallpaper; }
            set { Set(() => EnableWallpaper, ref enableWallpaper, value); }
        }

        public bool UseMasterPortraits
        {
            get { return useMasterPortraits; }
            set { Set(() => UseMasterPortraits, ref useMasterPortraits, value); }
        }

        public bool Downloading
        {
            get { return downloading; }
            set { Set(() => Downloading, ref downloading, value); }
        }

        public RelayCommand DownloadCommand => new RelayCommand(Download);

        public void SaveSettings()
        {
            if (string.IsNullOrWhiteSpace(BackgroundColorSetting)
                || string.IsNullOrWhiteSpace(TextColorSetting)
                || string.IsNullOrWhiteSpace(BorderColorSetting)) return;

            Settings.Default.BackgroundColor = BackgroundColorSetting;
            Settings.Default.TextColor = TextColorSetting;
            Settings.Default.BorderColor = BorderColorSetting;
            Settings.Default.WallpapersPath =
                EnableWallpaper && !string.IsNullOrWhiteSpace(WallpapersPath) && Directory.Exists(WallpapersPath)
                    ? WallpapersPath
                    : string.Empty;
            Settings.Default.UseMasterPortraits = UseMasterPortraits;

            Settings.Default.Save();
            messenger.Send(new SettingsSavedMessage());
            window.Close();
        }

        public void Download()
        {
            Task.Run(async () => await DownloadAsync());
        }

        public async Task DownloadAsync()
        {
            Downloading = true;
            try
            {
                await portraitDownloader.DownloadPortraits();
            }
            catch (Exception)
            {
                messageBoxWrapper.Show("Error downloading images");
            }
            Downloading = false;
        }
    }
}