using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace HotStats.Services
{
    public interface IHeroDataDownloader
    {
        Task DownloadData();
    }

    public class HeroDataDownloader : IHeroDataDownloader
    {
        private const string BaseUrl = "http://heroesofthestorm.wikia.com";
        private readonly IHeroDataRepository heroDataRepository;

        public HeroDataDownloader(IHeroDataRepository heroDataRepository)
        {
            this.heroDataRepository = heroDataRepository;
        }

        public async Task DownloadData()
        {
            var htmlDocument = await GetHtmlDocument();
            if (htmlDocument.DocumentNode == null) return;

            var contentNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='mw-content-text']");

            var links = new Dictionary<string, string>();

            var normalHeroPortraitDivs = contentNode.SelectSingleNode("div").SelectNodes("div");
            foreach (var normalHeroPortraitDiv in normalHeroPortraitDivs)
            {
                var divs = normalHeroPortraitDiv.SelectNodes("div");
                var aTag = divs.Last().SelectSingleNode(".//a");
                var link = aTag.GetAttributeValue("href", "");
                var name = aTag.InnerText;
                links.Add(name, link);
            }

            var heroes = new List<Hero>();
            foreach (var key in links.Keys)
            {
                var hero = await DownloadHeroData(key, links[key]);
                if (hero != null)
                    heroes.Add(hero);
            }

            var heroesJson = JsonConvert.SerializeObject(heroes);
            heroDataRepository.SaveData(heroes);
            File.WriteAllText(FilePaths.HeroData, heroesJson);
        }

        public async Task<Hero> DownloadHeroData(string name, string link)
        {
            try
            {
                var htmlDocument = new HtmlDocument();
                var responseMessage =
                    await WebClients.HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}{link}"));

                using (responseMessage)
                {
                    if (!responseMessage.IsSuccessStatusCode) return null;
                    var content = await responseMessage.Content.ReadAsStringAsync();
                    htmlDocument.LoadHtml(content);
                }

                var universeText =
                    htmlDocument.DocumentNode.SelectSingleNode(
                        "//div[contains(a/following-sibling::text(), 'is from the')]");
                var universe = universeText?.SelectSingleNode("i")?.FirstChild
                    ?.InnerText;
                if (universe == null)
                {
                    universeText =
                        htmlDocument.DocumentNode.SelectSingleNode(
                            "//div[contains(noscript/following-sibling::text(), 'is from the')]");
                    universe = universeText?.SelectSingleNode("i")?.FirstChild
                        ?.InnerText;
                }

                var roleText = htmlDocument.DocumentNode.SelectSingleNode("//h3//*[contains(., 'Role')]");
                var roleParent = roleText?.ParentNode?.ParentNode;

                var role = roleParent?.SelectSingleNode("div")?.SelectSingleNode("a")?.GetAttributeValue("title", "");

                var difficultyText = htmlDocument.DocumentNode.SelectSingleNode("//h3//*[contains(., 'Difficulty')]");
                var difficultyParent = difficultyText?.ParentNode?.ParentNode;

                var difficulty = difficultyParent?.SelectSingleNode("div")?.SelectSingleNode("span")?.InnerText;

                return new Hero
                {
                    Name = name,
                    Role = role,
                    Universe = universe,
                    Difficulty = difficulty
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<HtmlDocument> GetHtmlDocument()
        {
            var htmlDocument = new HtmlDocument();
            var responseMessage =
                await WebClients.HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                    $"{BaseUrl}/wiki/Progression_Portraits"));

            using (responseMessage)
            {
                if (!responseMessage.IsSuccessStatusCode) return htmlDocument;
                var content = await responseMessage.Content.ReadAsStringAsync();
                htmlDocument.LoadHtml(content);
            }
            return htmlDocument;
        }
    }

    public class Hero
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public string Universe { get; set; }
        public string Difficulty { get; set; }
    }
}