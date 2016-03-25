namespace HotStats.ViewModels.Interfaces
{
    public interface ISelectedHeroViewModel
    {
        bool HeroSelected { get; set; }
        string Hero { get; set; }
        ITotalStatsViewModel TotalStatsViewModel { get; set; }
    }
}