using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotStats.Services;
using Xunit;

namespace HotStats.Test.Services
{
    public class PortraitDownloaderTests
    {
        private PortraitDownloader _portraitDownloader;

        public PortraitDownloaderTests()
        {
            _portraitDownloader = new PortraitDownloader();
        }

        [Fact]
        public async Task Test()
        {
            await _portraitDownloader.DownloadPortraits();
        }
    }
}
