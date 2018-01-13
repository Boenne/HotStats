using System.Threading.Tasks;
using HotStats.Services;
using Xunit;

namespace HotStats.Test.Services
{
    public class HeroDataDownloaderTests
    {
        [Fact]
        public async Task Test()
        {
            var heroDataDownloader = new HeroDataDownloader(new HeroDataRepository());
            await heroDataDownloader.DownloadData();
        }
    }
}