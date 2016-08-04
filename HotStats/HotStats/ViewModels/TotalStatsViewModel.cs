using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.ReplayParser;
using HotStats.Services.Interfaces;
using HotStats.ViewModels.Interfaces;
using HotStats.Wrappers;

namespace HotStats.ViewModels
{
    public class TotalStatsViewModel : ObservableObject, ITotalStatsViewModel
    {
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly IReplayRepository replayRepository;
        private int assists;
        private int damageTaken;
        private int deaths;
        private int expContribution;
        private List<GameMode> gameModes = new List<GameMode> {GameMode.QuickMatch, GameMode.HeroLeague, GameMode.UnrankedDraft};
        private int games;
        private int healing;
        private string hero;
        private int heroDamage;
        private bool heroSelected;
        private string playerName;
        private int quickMatches;
        private int rankedGames;
        private int siegeDamage;
        private int takedowns;
        private int unranked;

        public TotalStatsViewModel(IMessenger messenger, IReplayRepository replayRepository,
            IDispatcherWrapper dispatcherWrapper)
        {
            this.replayRepository = replayRepository;
            this.dispatcherWrapper = dispatcherWrapper;
            messenger.Register<PlayerNameHasBeenSetMessage>(this, message =>
            {
                playerName = message.PlayerName;
                CalculateStatsAsync();
            });
            messenger.Register<GameModeChangedMessage>(this, message =>
            {
                gameModes = message.GameModes;
                CalculateStatsAsync();
            });
        }

        public TotalStatsViewModel()
        {
        }

        public bool HeroSelected
        {
            get { return heroSelected; }
            set
            {
                heroSelected = value;
                OnPropertyChanged();
            }
        }

        public string Hero
        {
            get { return hero; }
            set
            {
                hero = value;
                OnPropertyChanged();
            }
        }

        public int Games
        {
            get { return games; }
            set
            {
                games = value;
                OnPropertyChanged();
            }
        }

        public int RankedGames
        {
            get { return rankedGames; }
            set
            {
                rankedGames = value;
                OnPropertyChanged();
            }
        }

        public int QuickMatches
        {
            get { return quickMatches; }
            set
            {
                quickMatches = value;
                OnPropertyChanged();
            }
        }

        public int Unranked
        {
            get { return unranked; }
            set
            {
                unranked = value;
                OnPropertyChanged();
            }
        }

        public int Takedowns
        {
            get { return takedowns; }
            set
            {
                takedowns = value;
                OnPropertyChanged();
            }
        }

        public int Deaths
        {
            get { return deaths; }
            set
            {
                deaths = value;
                OnPropertyChanged();
            }
        }

        public int Assists
        {
            get { return assists; }
            set
            {
                assists = value;
                OnPropertyChanged();
            }
        }

        public int HeroDamage
        {
            get { return heroDamage; }
            set
            {
                heroDamage = value;
                OnPropertyChanged();
            }
        }

        public int SiegeDamage
        {
            get { return siegeDamage; }
            set
            {
                siegeDamage = value;
                OnPropertyChanged();
            }
        }

        public int Healing
        {
            get { return healing; }
            set
            {
                healing = value;
                OnPropertyChanged();
            }
        }

        public int DamageTaken
        {
            get { return damageTaken; }
            set
            {
                damageTaken = value;
                OnPropertyChanged();
            }
        }

        public int ExpContribution
        {
            get { return expContribution; }
            set
            {
                expContribution = value;
                OnPropertyChanged();
            }
        }

        public void CalculateStatsAsync()
        {
            Task.Factory.StartNew(() => dispatcherWrapper.BeginInvoke(CalculateStats));
        }

        public void CalculateStats()
        {
            ResetAll();
            var replays = replayRepository.GetReplays().Where(x => gameModes.Contains(x.GameMode));
            foreach (var replay in replays)
            {
                var player = replay.Players.FirstOrDefault(x => x.Name.ToLower() == playerName.ToLower());
                if (player == null) continue;
                Games++;
                switch (replay.GameMode)
                {
                    case GameMode.QuickMatch:
                        QuickMatches++;
                        break;
                    case GameMode.HeroLeague:
                        RankedGames++;
                        break;
                    case GameMode.UnrankedDraft:
                        Unranked++;
                        break;
                }
                if (!player.HasScoreResult()) continue;
                Takedowns += player.ScoreResult.SoloKills;
                Deaths += player.ScoreResult.Deaths;
                Assists += player.ScoreResult.Assists;
                HeroDamage += player.ScoreResult.HeroDamage;
                SiegeDamage += player.ScoreResult.SiegeDamage;
                Healing += player.ScoreResult.Healing ?? 0;
                DamageTaken += player.ScoreResult.DamageTaken ?? 0;
                ExpContribution += player.ScoreResult.ExperienceContribution;
            }
        }

        public void ResetAll()
        {
            Games = 0;
            RankedGames = 0;
            QuickMatches = 0;
            Takedowns = 0;
            Deaths = 0;
            Assists = 0;
            HeroDamage = 0;
            SiegeDamage = 0;
            Healing = 0;
            DamageTaken = 0;
            ExpContribution = 0;
        }
    }
}