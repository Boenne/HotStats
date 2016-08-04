namespace HotStats.ViewModels.Interfaces
{
    public interface ITotalStatsViewModel
    {
        int Games { get; set; }
        int RankedGames { get; set; }
        int QuickMatches { get; set; }
        int Unranked { get; set; }
        int Takedowns { get; set; }
        int Deaths { get; set; }
        int Assists { get; set; }
        int HeroDamage { get; set; }
        int SiegeDamage { get; set; }
        int Healing { get; set; }
        int DamageTaken { get; set; }
        int ExpContribution { get; set; }
    }
}