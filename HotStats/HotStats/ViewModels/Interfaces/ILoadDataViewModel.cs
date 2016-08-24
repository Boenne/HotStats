using GalaSoft.MvvmLight.Command;

namespace HotStats.ViewModels.Interfaces
{
    public interface ILoadDataViewModel
    {
        RelayCommand LoadDataCommand { get; }
        bool IsLoading { get; set; }
        int FilesProcessed { get; set; }
        int FileCount { get; set; }
        long ElapsedTime { get; set; }
        long ApproxTimeLeft { get; set; }
    }
}