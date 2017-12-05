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
        Task DownloadPortraits();
    }

    public class PortraitDownloader : IPortraitDownloader
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly WebClient WebClient = new WebClient();

        public async Task DownloadPortraits()
        {
            CreateFolders();

            var htmlDocument = await GetHtmlDocument();
            if (htmlDocument.DocumentNode == null) return;

            //This is needed to be able to download portraits if the protocol is https
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            
            var contentNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='mw-content-text']");

            var normalHeroPortraitDivs = contentNode.SelectSingleNode("div[@id='gallery-0']").SelectNodes("div");
            foreach (var normalHeroPortraitDiv in normalHeroPortraitDivs)
            {
                var divs = normalHeroPortraitDiv.SelectNodes("div");
                var img = divs[0].SelectSingleNode(".//img");
                var imgSrc = img.GetAttributeValue("src", null);
                var name = divs.Last().SelectSingleNode(".//a").InnerText;
                await DownloadPortrait(imgSrc, "normal", name);
            }

            var masterHeroPortraitDivs = contentNode.SelectSingleNode("div[@id='gallery-1']").SelectNodes("div");
            foreach (var masterHeroPortraitDiv in masterHeroPortraitDivs)
            {
                var divs = masterHeroPortraitDiv.SelectNodes("div");
                var img = divs[0].SelectSingleNode(".//img");
                var imgSrc = img.GetAttributeValue("src", null);
                var name = divs.Last().SelectSingleNode(".//a").InnerText;
                await DownloadPortrait(imgSrc, "master", name);
            }
        }

        public void Dispose()
        {
            HttpClient.Dispose();
            WebClient.Dispose();
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
            var directoryInfo = new DirectoryInfo($"{Environment.CurrentDirectory}/images/master");
            if (!directoryInfo.Exists)
                directoryInfo.Create();

            directoryInfo = new DirectoryInfo($"{Environment.CurrentDirectory}/images/normal");
            if (!directoryInfo.Exists)
                directoryInfo.Create();
        }

        public async Task DownloadPortrait(string url, string subFolderName, string name)
        {
            await Task.Run(() =>
            {
                try
                {
                    WebClient.DownloadFile(new Uri(url),
                        $"{Environment.CurrentDirectory}/Images/{subFolderName}/{name}.png");
                }
                catch (Exception e)
                {
                }
            });
        }

        ~PortraitDownloader()
        {
            Dispose();
        }
    }
}