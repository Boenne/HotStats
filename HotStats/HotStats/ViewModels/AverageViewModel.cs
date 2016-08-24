namespace HotStats.ViewModels
{
    public class AverageViewModel
    {
        public int Games { get; set; }
        public double TakeDowns { get; set; }
        public double Deaths { get; set; }
        public double Assists { get; set; }
        public double HeroDamage { get; set; }
        public double SiegeDamage { get; set; }
        public double Healing { get; set; }
        public double DamageTaken { get; set; }
        public double ExpContribution { get; set; }
        public long GameLength { get; set; }
        public int GamesWithScoreResults { get; set; }
        public string Title { get; set; }
        public bool WinningRow { get; set; }
    }
}