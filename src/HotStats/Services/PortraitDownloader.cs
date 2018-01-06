using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace HotStats.Services
{
    public interface IPortraitDownloader : IDisposable
    {
        Task<List<Portrait>> GetPortraits();
        Task DownloadPortraits(List<Portrait> portraits, bool overwrite = true);
    }

    public class PortraitDownloader : IPortraitDownloader
    {
        private const string FileExtension = "png";
        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly WebClient WebClient = new WebClient();
        private readonly string imageDirectory = $"{Environment.CurrentDirectory}/images";

        public async Task<List<Portrait>> GetPortraits()
        {
            var portraits = new List<Portrait>();
            var htmlDocument = await GetHtmlDocument();
            if (htmlDocument.DocumentNode == null) return portraits;

            //This is needed to be able to download portraits if the protocol is https
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                                                   SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var contentNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='mw-content-text']");

            var normalHeroPortraitDivs = contentNode.SelectSingleNode("div[@id='gallery-0']").SelectNodes("div");
            foreach (var normalHeroPortraitDiv in normalHeroPortraitDivs)
            {
                var divs = normalHeroPortraitDiv.SelectNodes("div");
                var img = divs[0].SelectSingleNode(".//img");
                var imgSrc = img.GetAttributeValue("src", null);
                var name = divs.Last().SelectSingleNode(".//a").InnerText;
                portraits.Add(new Portrait
                {
                    Name = name,
                    Source = imgSrc,
                    IsMaster = false
                });
            }

            var masterHeroPortraitDivs = contentNode.SelectSingleNode("div[@id='gallery-1']").SelectNodes("div");
            foreach (var masterHeroPortraitDiv in masterHeroPortraitDivs)
            {
                var divs = masterHeroPortraitDiv.SelectNodes("div");
                var img = divs[0].SelectSingleNode(".//img");
                var imgSrc = img.GetAttributeValue("src", null);
                var name = divs.Last().SelectSingleNode(".//a").InnerText;
                portraits.Add(new Portrait
                {
                    Name = name,
                    Source = imgSrc,
                    IsMaster = true
                });
            }

            return portraits;
        }

        public void Dispose()
        {
            HttpClient.Dispose();
            WebClient.Dispose();
        }

        public async Task DownloadPortraits(List<Portrait> portraits, bool overwrite = true)
        {
            CreateFolders();
            foreach (var portrait in portraits)
            {
                if (!overwrite && PortraitExists(portrait)) continue;
                await DownloadPortrait(portrait);
            }
        }

        public async Task<HtmlDocument> GetHtmlDocument()
        {
            var htmlDocument = new HtmlDocument();
            var responseMessage =
                await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                    "http://heroesofthestorm.wikia.com/wiki/Progression_Portraits"));
            using (responseMessage)
            {
                if (!responseMessage.IsSuccessStatusCode) return htmlDocument;
                var content = await responseMessage.Content.ReadAsStringAsync();
                htmlDocument.LoadHtml(content);
            }
            return htmlDocument;
        }

        public void CreateFolders()
        {
            var directoryInfo = new DirectoryInfo($"{imageDirectory}/master");
            if (!directoryInfo.Exists)
                directoryInfo.Create();

            directoryInfo = new DirectoryInfo($"{imageDirectory}/normal");
            if (!directoryInfo.Exists)
                directoryInfo.Create();
        }

        public async Task DownloadPortrait(Portrait portrait)
        {
            await Task.Run(() =>
            {
                try
                {
                    WebClient.DownloadFile(new Uri(portrait.Source),
                        $"{imageDirectory}/{portrait.SubFolder}/{portrait.Name}.{FileExtension}");
                }
                catch (Exception e)
                {
                }
            });
        }

        public bool PortraitExists(Portrait portrait)
        {
            return File.Exists($"{imageDirectory}/{portrait.SubFolder}/{portrait.Name}.{FileExtension}");
        }

        ~PortraitDownloader()
        {
            Dispose();
        }
    }

    public class Portrait
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public bool IsMaster { get; set; }
        public string SubFolder => IsMaster ? "master" : "normal";
    }
}