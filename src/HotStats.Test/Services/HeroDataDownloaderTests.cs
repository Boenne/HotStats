using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var heroDataDownloader = new HeroDataDownloader();
            await heroDataDownloader.DownloadData();
        }
    }
}
