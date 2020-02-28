using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

using HtmlAgilityPack;

namespace VillageListCrawler
{
    public class CommunePage : WikipediaPage
    {
        const string SentenceIdentifyingString = " composed of ";

        public List<string> Villages { get; private set; }

        protected override void Load()
        {
            string html = GetPageSource();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNodeCollection communeNodes = doc.DocumentNode.SelectNodes("//*[@id='mw-content-text']/div[1]/p");

            Villages = new List<string>();

            int nodeIndex = 0;
            foreach (HtmlNode node in communeNodes)
            {
                nodeIndex += 1;

                if (!node.InnerText.Contains(SentenceIdentifyingString) ||
                    !node.InnerText.Contains("village"))
                {
                    continue;
                }

                string sentence = node.InnerText.Split('.').FirstOrDefault(x => x.Contains(SentenceIdentifyingString));

                Regex countRegex = new Regex($".*{SentenceIdentifyingString}(.*) (village|villages)[:,]");
                string countStr = countRegex.Match(sentence).Groups[1].Value;

                Regex listRegex = new Regex($".*{SentenceIdentifyingString}{countStr} (village|villages)[:,] (.*)");
                string listStr = listRegex.Match(sentence).Groups[2].Value;

                // The villages are written in a list
                if (!string.IsNullOrEmpty(countStr) && string.IsNullOrEmpty(listStr))
                {
                    if (node.NextSibling.NextSibling.Name == "ul")
                    {
                        HtmlNodeCollection villagesUlItems = doc.DocumentNode.SelectNodes($"//*[@id='mw-content-text']/div[1]/p[{nodeIndex}]/following-sibling::ul[1]/li");

                        if (villagesUlItems != null)
                        {
                            listStr = string.Join(",", villagesUlItems.Select(x => x.InnerText.Replace(',', '/')));
                        }
                    }
                    // The villages are written in a table
                    else
                    {
                        HtmlNodeCollection villagesCells = doc.DocumentNode.SelectNodes($"//*[@id='mw-content-text']/div[1]/p[{nodeIndex}]/following-sibling::table[1]/tbody/tr/td[1]");

                        if (villagesCells != null)
                        {
                            listStr = string.Join(",", villagesCells.Select(x => x.InnerText));
                        }
                    }
                }

                // Something is not ok
                if (string.IsNullOrEmpty(countStr) || string.IsNullOrEmpty(listStr))
                {
                    break;
                    throw new Exception();
                }

                int count = StringNumberToInt(countStr);
                Villages = GetNamesFromListString(count, listStr);
            }
        }

        List<string> GetNamesFromListString(int count, string listString)
        {
            List<string> names = new List<string>();

            string cleanedList = Regex.Replace(listString, @"\((?:\([^()]*\)|[^()])*\)", "");
            cleanedList = Regex.Replace(cleanedList, @"\[[^\)]*\]", "");
            cleanedList = Regex.Replace(cleanedList, @"&#91;[0-9]*&#93;", "");
            cleanedList = Regex.Replace(cleanedList, @"[/;][ ]*[^,]*", "");
            cleanedList = Regex.Replace(cleanedList, @" -[ ]*[^,]*", "");
            cleanedList = Regex.Replace(cleanedList, @" and ([^,]*),.*, and ", ",$1,");
            cleanedList = Regex.Replace(cleanedList, @"(.* and [^,]*)(,.*)", "$1");
            cleanedList = Regex.Replace(cleanedList, @"(.* and [^,]*)( and .*)", "$1");
            cleanedList = Regex.Replace(cleanedList, @" along with ", " and ");
            cleanedList = Regex.Replace(cleanedList, @" and ", ",");
            cleanedList = Regex.Replace(cleanedList, @"^[ ]*", "");
            cleanedList = Regex.Replace(cleanedList, @"[ ]*$", "");
            cleanedList = Regex.Replace(cleanedList, @"[ ]*,[ ]*", ",");
            cleanedList = Regex.Replace(cleanedList, @"[ ]*,", ",");
            cleanedList = Regex.Replace(cleanedList, @"[ ]{2,}", " ");
            cleanedList = Regex.Replace(cleanedList, @"[,]{2,}", ",");

            names = cleanedList.Split(',').Select(x => x.Trim()).Take(count).ToList();

            if (listString.Contains("Topolog"))
            {
                Console.WriteLine("caca");
            }

            if (names.Count != count)
            {
                throw new ArgumentException("Count doesn't match");
            }

            return names;
        }
        int StringNumberToInt(string nr)
        {
            int val;
            bool canParse = int.TryParse(nr, out val);

            if (canParse)
            {
                return val;
            }

            if (nr == "one" || nr == "a single") return 1;
            if (nr == "two") return 2;
            if (nr == "three") return 3;
            if (nr == "four") return 4;
            if (nr == "five") return 5;
            if (nr == "six") return 6;
            if (nr == "seven") return 7;
            if (nr == "eight") return 8;
            if (nr == "nine") return 9;
            if (nr == "ten") return 10;
            if (nr == "eleven") return 11;
            if (nr == "twelve") return 12;
            if (nr == "thirteen") return 13;
            if (nr == "fourteen") return 14;
            if (nr == "fifteen") return 15;
            if (nr == "sixteen") return 16;
            if (nr == "seventeen") return 17;
            if (nr == "eighteen") return 18;
            if (nr == "nineteen") return 19;
            if (nr == "twenty") return 20;
            if (nr == "twenty-one") return 21;
            if (nr == "twenty-two") return 22;
            if (nr == "twenty-three") return 23;
            if (nr == "twenty-four") return 24;
            if (nr == "twenty-five") return 25;
            if (nr == "twenty-six") return 26;
            if (nr == "twenty-seven") return 27;
            if (nr == "twenty-eight") return 28;
            if (nr == "twenty-nine") return 29;
            if (nr == "thirty") return 30;
            if (nr == "thirty-one") return 31;
            if (nr == "thirty-two") return 32;
            if (nr == "thirty-three") return 33;
            if (nr == "thirty-four") return 34;
            if (nr == "thirty-five") return 35;
            if (nr == "thirty-six") return 36;
            if (nr == "thirty-seven") return 37;
            if (nr == "thirty-eight") return 38;
            if (nr == "thirty-nine") return 39;
            if (nr == "forty") return 40;

            throw new ArgumentException($"Invalid or unrecognised number string '{nr}'");
        }
    }
}
