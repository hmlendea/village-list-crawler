using System;
using System.Collections.Generic;
using System.IO;

namespace VillageListCrawler
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Console.WriteLine($"═╦═══ Getting the counties)");
            CountryPage countryPage = new CountryPage();
            countryPage.Load("https://en.wikipedia.org/wiki/Category:Communes_and_villages_in_Romania");

            List<string> blacklistedCommunes = new List<string>
            {
                "Corund"
            };

            if (!Directory.Exists("output"))
            {
                Directory.CreateDirectory("output");
            }

            for (int countyIndex = 37; countyIndex < countryPage.Counties.Count; countyIndex++)
            {
                WikipediaLink countyLink = countryPage.Counties[countyIndex];

                if (!Directory.Exists(countyLink.Name))
                {
                    Directory.CreateDirectory($"output/{countyLink.Name}");
                }

                if (countyIndex < countryPage.Counties.Count - 1)
                {
                    Console.WriteLine($" ╠╦══ Getting the communes in {countyLink.Name}");
                }
                else
                {
                    Console.WriteLine($" ╠╚══ Getting the communes in {countyLink.Name}");
                }

                CountyPage countyPage = new CountyPage();
                countyPage.Load(countyLink.Link);

                for (int communeIndex = 0; communeIndex < countyPage.Communes.Count; communeIndex++)
                {
                    WikipediaLink communeLink = countyPage.Communes[communeIndex];

                    string fileName = $"output/{countyLink.Name}/{communeLink.Name}.txt";

                    if (communeLink.Name.Contains(".") ||
                        blacklistedCommunes.Contains(communeLink.Name))
                    {
                        Console.WriteLine($" ║    NOT LOADING FOR {communeLink.Name}");
                        File.WriteAllText(fileName, "CANNOT GET LIST FOR THIS COMMUNE");
                        continue;
                    }

                    if (communeIndex < countyPage.Communes.Count - 1)
                    {
                        Console.WriteLine($" ║╠══ Getting the villages in {communeLink.Name}");
                    }
                    else
                    {
                        Console.WriteLine($" ║╚══ Getting the villages in {communeLink.Name}");
                    }

                    CommunePage communePage = new CommunePage();
                    communePage.Load(communeLink.Link);

                    File.WriteAllLines(fileName, communePage.Villages);
                }
            }

            Console.ReadKey();
        }
    }
}
