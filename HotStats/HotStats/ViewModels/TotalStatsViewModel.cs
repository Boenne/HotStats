using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.ReplayParser;
using HotStats.Services.Interfaces;
using HotStats.ViewModels.Interfaces;
using HotStats.Wrappers;

namespace HotStats.ViewModels
{
    public class TotalStatsViewModel : ViewModelBase, ITotalStatsViewModel
    {
        private readonly IDispatcherWrapper dispatcherWrapper;
        private readonly IReplayRepository replayRepository;
        private int assists;
        private int damageTaken;
        private int deaths;
        private int expContribution;

        private List<GameMode> gameModes = new List<GameMode>
        {
            GameMode.QuickMatch,
            GameMode.HeroLeague,
            GameMode.UnrankedDraft
        };

        private int games;
        private int healing;
        private string hero;
        private int heroDamage;
        private bool heroSelected;
        private string playerName;
        private int quickMatches;
        private int rankedGames;
        private DateTime selectedDateFilter;
        private string selectedHero;
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
            messenger.Register<DataHasBeenRefreshedMessage>(this, message => { CalculateStatsAsync(); });
            messenger.Register<DateFilterSelectedMessage>(this, message =>
            {
                selectedDateFilter = message.Date;
                CalculateStatsAsync();
            });
            messenger.Register<HeroSelectedMessage>(this, message =>
            {
                selectedHero = message.Hero;
                CalculateStatsAsync();
            });
            messenger.Register<HeroDeselectedMessage>(this, message =>
            {
                selectedHero = null;
                CalculateStatsAsync();
            });
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

        public int Games
        {
            get { return games; }
            set { Set(() => Games, ref games, value); }
        }

        public int RankedGames
        {
            get { return rankedGames; }
            set { Set(() => RankedGames, ref rankedGames, value); }
        }

        public int QuickMatches
        {
            get { return quickMatches; }
            set { Set(() => QuickMatches, ref quickMatches, value); }
        }

        public int Unranked
        {
            get { return unranked; }
            set { Set(() => Unranked, ref unranked, value); }
        }

        public int Takedowns
        {
            get { return takedowns; }
            set { Set(() => Takedowns, ref takedowns, value); }
        }

        public int Deaths
        {
            get { return deaths; }
            set { Set(() => Deaths, ref deaths, value); }
        }

        public int Assists
        {
            get { return assists; }
            set { Set(() => Assists, ref assists, value); }
        }

        public int HeroDamage
        {
            get { return heroDamage; }
            set { Set(() => HeroDamage, ref heroDamage, value); }
        }

        public int SiegeDamage
        {
            get { return siegeDamage; }
            set { Set(() => SiegeDamage, ref siegeDamage, value); }
        }

        public int Healing
        {
            get { return healing; }
            set { Set(() => Healing, ref healing, value); }
        }

        public int DamageTaken
        {
            get { return damageTaken; }
            set { Set(() => DamageTaken, ref damageTaken, value); }
        }

        public int ExpContribution
        {
            get { return expContribution; }
            set { Set(() => ExpContribution, ref expContribution, value); }
        }

        public void CalculateStatsAsync()
        {
            Task.Factory.StartNew(() => dispatcherWrapper.BeginInvoke(CalculateStats));
        }

        public void CalculateStats()
        {
            ResetAll();
            var replays =
                replayRepository.GetReplays()
                    .Where(x => gameModes.Contains(x.GameMode) && x.Timestamp >= selectedDateFilter);
            foreach (var replay in replays)
            {
                var player = selectedHero != null
                    ? replay.Players.FirstOrDefault(
                        x => x.Name.ToLower() == playerName.ToLower() && x.Character == selectedHero)
                    : replay.Players.FirstOrDefault(x => x.Name.ToLower() == playerName.ToLower());
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
            Unranked = 0;
        }
    }
}