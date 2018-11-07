using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;

namespace HotStats.ViewModels
{
    public class FiltersViewModel : ViewModelBase, IFiltersViewModel
    {
        private readonly IMessenger messenger;
        private string selectedMap;

        public FiltersViewModel(IMessenger messenger) : base(messenger)
        {
            this.messenger = messenger;
        }

        public string SelectedMap
        {
            get => selectedMap;
            set { Set(() => SelectedMap, ref selectedMap, value); }
        }
        
        public RelayCommand SelectMapCommand => new RelayCommand(SelectMap);
        public RelayCommand<string> FilterGameModeCommand => new RelayCommand<string>(FilterGameMode);

        public void SelectMap()
        {

        }

        public void FilterGameMode(string gameMode)
        {
            
        }

        public Func<string, Task> FilterUniverse => universe =>
        {
            return Task.Run(() =>
            {

            });
        };

        public Func<string, Task> FilterClass => @class =>
        {
            return Task.Run(() =>
            {

            });
        };
    }

    public interface IFiltersViewModel
    {
        RelayCommand SelectMapCommand { get; }
        RelayCommand<string> FilterGameModeCommand { get; }
        Func<string, Task> FilterUniverse { get; }
        Func<string, Task> FilterClass { get; }
    }
}