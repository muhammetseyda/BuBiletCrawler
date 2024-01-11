using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    internal class hizCrawler : IDisposable
    {
        public List<Product> urunler = new List<Product>();
        public class Product
        {
            public string Dealer { get; set; }
            public string Name { get; set; }
            public double PriceListe { get; set; }
            public double PriceIndirimli { get; set; }
        }
        public hizCrawler(string key)
        {
            key = key.Replace("\n", "");
            key = key.Replace(" ", "%20");
            var httpClient = new HttpClient();

            var response = httpClient.GetAsync("https://onlinesatis.hizyayinlari.com/Arama?1&kelime=" + key).Result;
            var contents = response.Content.ReadAsStringAsync().Result;

            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(contents);
            HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'ProductListContent')]");
            if (htmlNodes != null)
            {
                foreach (var node in htmlNodes)
                {
                    urunler.Add(new Product
                    {
                        PriceListe = Helpers.Price2(node.SelectSingleNode(".//div[contains(@class, 'regularPrice')]").InnerText),
                        PriceIndirimli = Helpers.Price2(node.SelectSingleNode(".//div[contains(@class, 'discountPrice')]").InnerText)
                    });
                }
            }

        }

        public double getMinPriceListe()
        {
            return urunler.OrderBy(x => x.PriceListe).First().PriceListe;
        }
        public double getMinPriceIndirimli()
        {
            return urunler.OrderBy(x => x.PriceIndirimli).First().PriceIndirimli;
        }

        public void Dispose()
        {
            urunler.Clear();
        }
    }
}
