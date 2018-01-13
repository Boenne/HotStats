using System.Collections.Generic;

namespace HotStats.Services
{
    public interface IHeroDataRepository
    {
        List<Hero> GetData();
        void SaveData(List<Hero> heroes);
    }

    public class HeroDataRepository : IHeroDataRepository
    {
        private List<Hero> heroes = new List<Hero>();

        public List<Hero> GetData()
        {
            return heroes;
        }

        public void SaveData(List<Hero> heroes)
        {
            this.heroes = heroes;
        }
    }
}