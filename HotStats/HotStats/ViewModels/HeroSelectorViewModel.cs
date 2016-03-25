using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.ReplayParser;
using HotStats.Services.Interfaces;
using HotStats.ViewModels.Interfaces;
using Newtonsoft.Json;

namespace HotStats.ViewModels
{
    public class HeroSelectorViewModel : ObservableObject, IHeroSelectorViewModel
    {
        private readonly IMessenger messenger;
        private readonly IReplayRepository replayRepository;
        private List<string> heroes;
        private bool playerNameIsSet;

        public HeroSelectorViewModel(IMessenger messenger, IReplayRepository replayRepository)
        {
            this.messenger = messenger;
            this.replayRepository = replayRepository;
            messenger.Register<SetPlayerNameMessage>(this, message => GetHeroesAsync(message.PlayerName));
        }

        public List<string> Heroes
        {
            get { return heroes; }
            set
            {
                heroes = value;
                OnPropertyChanged();
            }
        }

        public bool PlayerNameIsSet
        {
            get { return playerNameIsSet; }
            set
            {
                playerNameIsSet = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectHeroCommand => new RelayCommand<string>(SelectHero);

        public void SelectHero(string hero)
        {
            messenger.Send(new HeroSelectedMessage(hero));
        }

        public void GetHeroesAsync(string playerName)
        {
            Task.Factory.StartNew(() => GetHeroes(playerName));
        }

        public void GetHeroes(string playerName)
        {
            PlayerNameIsSet = true;
            var replays = replayRepository.GetReplays();
            if (replays == null)
            {
                var path = Environment.CurrentDirectory + "/data.txt";
                if (!File.Exists(path) || string.IsNullOrEmpty(playerName)) return;
                replays = JsonConvert.DeserializeObject<List<Replay>>(File.ReadAllText(path));
                replayRepository.SaveReplays(replays);
                messenger.Send(new DataHasBeenLoadedMessage());
            }
            var result = new Dictionary<string, int>();
            foreach (var replay in replays.Where(x => x != null))
            {
                var player = replay.Players.FirstOrDefault(x => x.Name.ToLower() == playerName.ToLower());
                if (player == null ) continue;
                if (result.ContainsKey(player.Character))
                    result[player.Character]++;
                else
                    result.Add(player.Character, 1);
            }
            Heroes = result.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
        }
    }
}