using System.IO;
using GalaSoft.MvvmLight;
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
        string BackgroundPath { get; set; }
    }

    public class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly IMessenger messenger;
        private string backgroundPath;
        private IClosable window;

        public SettingsViewModel(IMessenger messenger)
        {
            this.messenger = messenger;
        }

        public RelayCommand<IClosable> LoadedCommand => new RelayCommand<IClosable>(window =>
        {
            this.window = window;
            BackgroundPath = Settings.Default.BackgroundPath;
        });

        public RelayCommand SaveSettingsCommand => new RelayCommand(SaveSettings);

        public string BackgroundPath
        {
            get { return backgroundPath; }
            set { Set(() => BackgroundPath, ref backgroundPath, value); }
        }

        public void SaveSettings()
        {
            if (!File.Exists(BackgroundPath)) return;
            Settings.Default.BackgroundPath = BackgroundPath;
            Settings.Default.Save();
            messenger.Send(new SettingsSavedMessage());
            window.Close();
        }
    }
}