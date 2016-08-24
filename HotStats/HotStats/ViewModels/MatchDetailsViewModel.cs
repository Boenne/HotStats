﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotStats.Messaging;
using HotStats.Messaging.Messages;
using HotStats.Services.Interfaces;
using HotStats.ViewModels.Interfaces;

namespace HotStats.ViewModels
{
    public class MatchDetailsViewModel : ObservableObject, IMatchDetailsViewModel
    {
        private readonly IReplayRepository replayRepository;
        private List<PlayerViewModel> players;

        public MatchDetailsViewModel(IMessenger messenger, IReplayRepository replayRepository)
        {
            this.replayRepository = replayRepository;
            messenger.Register<MatchSelectedMessage>(this, message => GetDetailsAsync(message.Timestamp));
        }

        public List<PlayerViewModel> Players
        {
            get { return players; }
            set
            {
                players = value;
                OnPropertyChanged();
            }
        }

        public void GetDetailsAsync(DateTime timestamp)
        {
            Task.Factory.StartNew(() => GetDetails(timestamp));
        }

        public void GetDetails(DateTime timestamp)
        {
            var replay = replayRepository.GetReplays().FirstOrDefault(x => x.Timestamp == timestamp);
            if (replay == null) return;
            var playerViewModels = replay.Players.Select(x => new PlayerViewModel
            {
                Assists = x.ScoreResult.Assists,
                DamageTaken = x.ScoreResult.DamageTaken,
                Deaths = x.ScoreResult.Deaths,
                ExpContribution = x.ScoreResult.ExperienceContribution,
                Healing = x.ScoreResult.Healing,
                Hero = x.Character,
                HeroDamage = x.ScoreResult.HeroDamage,
                PlayerName = x.Name,
                SiegeDamage = x.ScoreResult.SiegeDamage,
                TakeDowns = x.ScoreResult.SoloKills,
                Winner = x.IsWinner
            }).ToList();
            Players = playerViewModels;
        }
    }
}