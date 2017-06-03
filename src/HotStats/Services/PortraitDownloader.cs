using System;
using System.Collections.Generic;
using System.IO;
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

            var previousHeroes = new List<string>();
            var selectSingleNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='mw-content-text']");
            var heroTables = selectSingleNode.SelectNodes("table");
            foreach (var heroTable in heroTables)
            {
                var rows = heroTable.SelectNodes("tr");
                var imageColumns = rows[0].SelectNodes("td");
                var nameColumns = rows[1].SelectNodes("td");
                for (var i = 0; i < imageColumns.Count; i++)
                {
                    var imgNode = imageColumns[i].SelectSingleNode("a");
                    var imgSrc = imgNode.GetAttributeValue("href", null);
                    var name = nameColumns[i].SelectSingleNode("a").InnerText;
                    if (previousHeroes.Contains(name))
                        await DownloadPortrait(imgSrc, "master", name);
                    else
                    {
                        previousHeroes.Add(name);
                        await DownloadPortrait(imgSrc, "normal", name);
                    }
                }
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