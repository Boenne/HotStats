using System;

namespace HotStats.Navigation
{
    public class NavigationFrames
    {
        public static NavigationFrame LoadData
            => new NavigationFrame
            {
                Name = "LoadData",
                Path = new Uri("../UserControls/LoadDataUserControl.xaml", UriKind.Relative)
            };

        public static NavigationFrame SetPlayerName
            => new NavigationFrame
            {
                Name = "SetPlayerName",
                Path = new Uri("../UserControls/SetPlayerNameUserControl.xaml", UriKind.Relative)
            };

        public static NavigationFrame MainPage
            => new NavigationFrame
            {
                Name = "MainPage",
                Path = new Uri("../UserControls/MainPageUserControl.xaml", UriKind.Relative)
            };

        public static NavigationFrame DownloadPortraits
            => new NavigationFrame
            {
                Name = "DownloadPortraits",
                Path = new Uri("../UserControls/DownloadPortraitsUserControl.xaml", UriKind.Relative)
            };
    }

    public class NavigationFrame
    {
        public string Name { get; set; }
        public Uri Path { get; set; }
    }
}