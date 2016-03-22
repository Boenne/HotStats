using System;
using HotStats.ReplayParser;

namespace HotStats.ViewModels
{
    public class MatchViewModel
    {
        public string Hero { get; set; }
        public string Map { get; set; }
        public bool Winner { get; set; }
        public int TakeDowns { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public int? Healing { get; set; }
        public int? DamageTaken { get; set; }
        public int SiegeDamage { get; set; }
        public int HeroDamage { get; set; }
        public GameMode GameMode { get; set; }
        public DateTime TimeStamp { get; set; }
        public TimeSpan GameLength { get; set; }
    }
}