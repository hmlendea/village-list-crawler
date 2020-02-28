using System;
using System.Collections.Generic;

using HtmlAgilityPack;

namespace VillageListCrawler
{
    public class CountryPage : WikipediaPage
    {
        public List<WikipediaLink> Counties { get; private set; }

        protected override void Load()
        {
            string html = GetPageSource();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNodeCollection communeNodes = doc.DocumentNode.SelectNodes("//*[@id='mw-subcategories']/div/div/div/ul/li/div/div[1]/a");
            Counties = new List<WikipediaLink>();

            foreach (HtmlNode node in communeNodes)
            {
                if (!node.InnerText.StartsWith("Communes in", StringComparison.InvariantCulture))
                {
                    continue;
                }

                int sasa = node.InnerText.LastIndexOf(' ');

                WikipediaLink link = new WikipediaLink();
                link.Name = node.InnerText.Substring(12, node.InnerText.LastIndexOf(' ') - 12);
                link.Link = $"https://en.wikipedia.org{node.Attributes[1].Value}";

                Counties.Add(link);
            }
        }
    }
}
