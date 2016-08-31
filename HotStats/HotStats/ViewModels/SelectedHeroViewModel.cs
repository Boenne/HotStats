using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Messaging.Messages;

namespace HotStats.ViewModels
{
    public class SelectedHeroViewModel : ViewModelBase, ISelectedHeroViewModel
    {
        private readonly IMessenger messenger;
        private string hero;
        private bool heroSelected;

        public SelectedHeroViewModel(IMessenger messenger)
        {
            this.messenger = messenger;
            messenger.Register<HeroSelectedMessage>(this, message =>
            {
                HeroSelected = true;
                Hero = message.Hero;
            });
            messenger.Register<HeroDeselectedMessage>(this, message => { HeroSelected = false; });
        }

        public bool HeroSelected
        {
            get { return heroSelected; }
            set { Set(() => HeroSelected, ref heroSelected, value); }
        }

        public string Hero
        {
            get { return hero; }
            set { Set(() => Hero, ref hero, value); }
        }

        public RelayCommand DeselectHeroCommand => new RelayCommand(DeselectHero);

        public void DeselectHero()
        {
            messenger.Send(new HeroDeselectedMessage());
        }
    }

    public interface ISelectedHeroViewModel
    {
        bool HeroSelected { get; set; }
        string Hero { get; set; }
        RelayCommand DeselectHeroCommand { get; }
    }
}