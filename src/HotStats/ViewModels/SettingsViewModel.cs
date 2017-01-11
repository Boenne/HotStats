using System;
using System.IO;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Properties;
using HotStats.Windows;

namespace HotStats.ViewModels
{
    public interface ISettingsViewModel
    {
        RelayCommand<IClosable> LoadedCommand { get; }
        RelayCommand SaveSettingsCommand { get; }
        string WallpapersPath { get; set; }
        string BackgroundColorSetting { get; set; }
        string TextColorSetting { get; set; }
        string BorderColorSetting { get; set; }
        bool EnableWallpaper { get; set; }
    }

    public class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly IMessenger messenger;
        private string backgroundColorSetting;
        private string borderColorSetting;
        private bool enableWallpaper;
        private string textColorSetting;
        private string wallpapersPath;
        private IClosable window;

        public SettingsViewModel(IMessenger messenger) : base(messenger)
        {
            this.messenger = messenger;
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

            Settings.Default.Save();
            messenger.Send(new SettingsSavedMessage());
            window.Close();
        }
    }
}