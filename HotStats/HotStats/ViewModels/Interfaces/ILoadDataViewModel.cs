using System.Windows.Input;

namespace HotStats.ViewModels.Interfaces
{
    public interface ILoadDataViewModel
    {
        ICommand LoadDataCommand { get; }
        bool IsLoading { get; set; }
        bool HasLoaded { get; set; }
        int FilesProcessed { get; set; }
        int FileCount { get; set; }
        long ElapsedTime { get; set; }
        long ApproxTimeLeft { get; set; }
        string PlayerName { get; set; }
        bool PlayerNameIsSet { get; set; }
        ICommand SetPlayerNameCommand { get; }
        ICommand LoadedCommand { get; }
    }
}