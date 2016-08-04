namespace HotStats.ViewModels
{
    public class PlayerViewModel
    {
        public string Hero { get; set; }
        public string PlayerName { get; set; }
        public bool Winner { get; set; }
        public int TakeDowns { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public int? Healing { get; set; }
        public int? DamageTaken { get; set; }
        public int SiegeDamage { get; set; }
        public int HeroDamage { get; set; }
        public int ExpContribution { get; set; }
    }
}