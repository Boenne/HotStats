using System.Threading.Tasks;
using HotStats.Services;
using Xunit;

namespace HotStats.Test.Services
{
    public class PortraitDownloaderTests
    {
        public PortraitDownloaderTests()
        {
            portraitDownloader = new PortraitDownloader();
        }

        private readonly PortraitDownloader portraitDownloader;

        [Fact]
        public async Task Test()
        {
            await portraitDownloader.GetPortraits();
        }
    }
}