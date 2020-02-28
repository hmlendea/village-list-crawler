using System.IO;
using System.Net;

namespace VillageListCrawler
{
    public abstract class WikipediaPage
    {
        public string URL { get; protected set; }

        public void Load(string url)
        {
            URL = url;
            Load();
        }

        protected abstract void Load();

        protected string GetPageSource()
        {
            WebRequest request = WebRequest.Create(URL);
            request.Method = "GET";

            WebResponse response = request.GetResponse();
            string html = string.Empty;

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                html = reader.ReadToEnd();
            }

            return html;
        }
    }
}
