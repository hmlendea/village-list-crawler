using System.Collections.Generic;

using HtmlAgilityPack;

namespace VillageListCrawler
{
    public class CountyPage : WikipediaPage
    {
        public List<WikipediaLink> Communes { get; private set; }

        protected override void Load()
        {
            string html = GetPageSource();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNodeCollection communeNodes = doc.DocumentNode.SelectNodes("//*[@id='mw-pages']/div/div/div/ul/li/a");
            Communes = new List<WikipediaLink>();

            foreach (HtmlNode node in communeNodes)
            {
                int length = node.InnerText.Length;

                if (node.InnerText.Contains(","))
                {
                    length = node.InnerText.IndexOf(',');
                }

                WikipediaLink link = new WikipediaLink();
                link.Name = node.InnerText.Substring(0, length);
                link.Link = $"https://en.wikipedia.org{node.Attributes[0].Value}";

                Communes.Add(link);
            }
        }
    }
}
