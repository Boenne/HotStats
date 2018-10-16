using System.Diagnostics;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Navigation;
using HotStats.Services;

namespace HotStats.ViewModels
{
    public class CheckVersionViewModel : ViewModelBase, ICheckVersionViewModel
    {
        private readonly INavigationService navigationService;
        private readonly IVersionChecker versionChecker;
        private bool isOutDated;

        public CheckVersionViewModel(IMessenger messenger, INavigationService navigationService,
            IVersionChecker versionChecker) : base(messenger)
        {
            this.navigationService = navigationService;
            this.versionChecker = versionChecker;
        }

        public bool IsOutDated
        {
            get => isOutDated;
            set { Set(() => IsOutDated, ref isOutDated, value); }
        }

        public RelayCommand ContinueCommand => new RelayCommand(Continue);
        public RelayCommand DownloadNewVersionCommand => new RelayCommand(DownloadNewVersion);
        public RelayCommand LoadedCommand => new RelayCommand(() => IsVersionOutDated());

        public async Task IsVersionOutDated()
        {
            if (await versionChecker.IsVersionOutdated())
                IsOutDated = true;
            else
                navigationService.NavigateTo(NavigationFrames.DownloadPortraits);
        }

        public void DownloadNewVersion()
        {
            Process.Start("https://github.com/Boenne/HotStats/raw/master/Program/Program.zip");
        }

        public void Continue()
        {
            navigationService.NavigateTo(NavigationFrames.DownloadPortraits);
        }
    }

    public interface ICheckVersionViewModel
    {
        bool IsOutDated { get; set; }
        RelayCommand ContinueCommand { get; }
        RelayCommand DownloadNewVersionCommand { get; }
        RelayCommand LoadedCommand { get; }
    }
}