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
        RelayCommand DownloadPortraitsCommand { get; }
        RelayCommand DownloadDataCommand { get; }
        string WallpapersPath { get; set; }
        string BackgroundColorSetting { get; set; }
        string TextColorSetting { get; set; }
        string BorderColorSetting { get; set; }
        string AccountName { get; set; }
        bool EnableWallpaper { get; set; }
        bool UseMasterPortraits { get; set; }
        bool DownloadingPortraits { get; set; }
        bool DownloadingData { get; set; }
    }

    public class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly IHeroDataDownloader heroDataDownloader;
        private readonly IMessageBoxWrapper messageBoxWrapper;
        private readonly IMessenger messenger;
        private readonly IPortraitDownloader portraitDownloader;
        private string accountName;
        private string backgroundColorSetting;
        private string borderColorSetting;
        private bool downloadingData;
        private bool downloadingPortraits;
        private bool enableWallpaper;
        private string textColorSetting;
        private bool useMasterPortraits = Settings.Default.UseMasterPortraits;
        private string wallpapersPath;
        private IClosable window;

        public SettingsViewModel(IMessenger messenger, IPortraitDownloader portraitDownloader,
            IMessageBoxWrapper messageBoxWrapper, IHeroDataDownloader heroDataDownloader) : base(messenger)
        {
            this.messenger = messenger;
            this.portraitDownloader = portraitDownloader;
            this.messageBoxWrapper = messageBoxWrapper;
            this.heroDataDownloader = heroDataDownloader;
        }

        public RelayCommand<IClosable> LoadedCommand => new RelayCommand<IClosable>(window =>
        {
            this.window = window;

            WallpapersPath = Settings.Default.WallpapersPath;
            BackgroundColorSetting = Settings.Default.BackgroundColor;
            TextColorSetting = Settings.Default.TextColor;
            BorderColorSetting = Settings.Default.BorderColor;
            AccountName = Settings.Default.PlayerName;

            EnableWallpaper = !string.IsNullOrEmpty(Settings.Default.WallpapersPath);
        });

        public RelayCommand SaveSettingsCommand => new RelayCommand(SaveSettings);

        public string WallpapersPath
        {
            get => wallpapersPath;
            set { Set(() => WallpapersPath, ref wallpapersPath, value); }
        }

        public string BackgroundColorSetting
        {
            get => backgroundColorSetting;
            set { Set(() => BackgroundColorSetting, ref backgroundColorSetting, value); }
        }

        public string TextColorSetting
        {
            get => textColorSetting;
            set { Set(() => TextColorSetting, ref textColorSetting, value); }
        }

        public string BorderColorSetting
        {
            get => borderColorSetting;
            set { Set(() => BorderColorSetting, ref borderColorSetting, value); }
        }

        public string AccountName
        {
            get => accountName;
            set { Set(() => AccountName, ref accountName, value); }
        }

        public bool EnableWallpaper
        {
            get => enableWallpaper;
            set { Set(() => EnableWallpaper, ref enableWallpaper, value); }
        }

        public bool UseMasterPortraits
        {
            get => useMasterPortraits;
            set { Set(() => UseMasterPortraits, ref useMasterPortraits, value); }
        }

        public bool DownloadingPortraits
        {
            get => downloadingPortraits;
            set { Set(() => DownloadingPortraits, ref downloadingPortraits, value); }
        }

        public bool DownloadingData
        {
            get => downloadingData;
            set { Set(() => DownloadingData, ref downloadingData, value); }
        }

        public RelayCommand DownloadPortraitsCommand => new RelayCommand(DownloadPortraits);
        public RelayCommand DownloadDataCommand => new RelayCommand(DownloadData);

        public void SaveSettings()
        {
            if (string.IsNullOrWhiteSpace(BackgroundColorSetting)
                || string.IsNullOrWhiteSpace(TextColorSetting)
                || string.IsNullOrWhiteSpace(BorderColorSetting)
                || string.IsNullOrWhiteSpace(AccountName)) return;

            Settings.Default.BackgroundColor = BackgroundColorSetting;
            Settings.Default.TextColor = TextColorSetting;
            Settings.Default.BorderColor = BorderColorSetting;
            Settings.Default.WallpapersPath =
                EnableWallpaper && !string.IsNullOrWhiteSpace(WallpapersPath) && Directory.Exists(WallpapersPath)
                    ? WallpapersPath
                    : string.Empty;
            Settings.Default.UseMasterPortraits = UseMasterPortraits;
            Settings.Default.PlayerName = AccountName;

            Settings.Default.Save();
            messenger.Send(new SettingsSavedMessage());
            window.Close();
        }

        public void DownloadPortraits()
        {
            Task.Run(async () => await DownloadPortraitsAsync());
        }

        public async Task DownloadPortraitsAsync()
        {
            DownloadingPortraits = true;
            try
            {
                var portraits = await portraitDownloader.GetPortraits();
                await portraitDownloader.DownloadPortraits(portraits);
            }
            catch (Exception)
            {
                messageBoxWrapper.Show("Error downloading portraits");
            }
            DownloadingPortraits = false;
        }

        public void DownloadData()
        {
            Task.Run(async () => await DownloadDataAsync());
        }

        public async Task DownloadDataAsync()
        {
            DownloadingData = true;
            try
            {
                await heroDataDownloader.DownloadData();
            }
            catch (Exception)
            {
                messageBoxWrapper.Show("Error downloading data");
            }
            DownloadingData = false;
        }
    }
}