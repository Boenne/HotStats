using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Properties;

namespace HotStats.ViewModels
{
    public abstract class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        private string backgroundColor = Settings.Default.BackgroundColor;
        private string borderColor = Settings.Default.BorderColor;
        private string textColor = Settings.Default.TextColor;

        protected ViewModelBase(IMessenger messenger)
        {
            messenger.Register<SettingsSavedMessage>(this, message =>
            {
                BackgroundColor = Settings.Default.BackgroundColor;
                TextColor = Settings.Default.TextColor;
                BorderColor = Settings.Default.BorderColor;
            });
        }

        public string BackgroundColor
        {
            get { return backgroundColor; }
            set { Set(() => BackgroundColor, ref backgroundColor, value); }
        }

        public string TextColor
        {
            get { return textColor; }
            set { Set(() => TextColor, ref textColor, value); }
        }

        public string BorderColor
        {
            get { return borderColor; }
            set { Set(() => BorderColor, ref borderColor, value); }
        }
    }
}