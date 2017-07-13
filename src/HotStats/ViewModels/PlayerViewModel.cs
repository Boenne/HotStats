namespace HotStats.ViewModels
{
    public class PlayerViewModel
    {
        public string Hero { get; set; }
        public PlayerVM Player { get; set; }
        public bool Winner { get; set; }
        public int TakeDowns { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public Stat Healing { get; set; }
        public Stat DamageTaken { get; set; }
        public Stat SiegeDamage { get; set; }
        public Stat HeroDamage { get; set; }
        public Stat ExpContribution { get; set; }
        public int Team { get; set; }
    }

    public class Stat
    {
        public int? Value { get; set; }
        public bool IsHighest { get; set; }
    }

    public class PlayerVM
    {
        public string Name { get; set; }
        public bool IsMe { get; set; }
    }
}