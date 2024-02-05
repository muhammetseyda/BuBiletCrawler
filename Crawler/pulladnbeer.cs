using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    public class pulladnbeer
    {
        public List<Product> urunler = new List<Product>();
        public class Product
        {
            public string Dealer { get; set; }
            public string Name { get; set; }
            public double PriceListe { get; set; }
            public double PriceIndirimli { get; set; }
        }
        public pulladnbeer()
        {
            
            var httpClient = new HttpClient();

            var response = httpClient.GetAsync("https://www.pullandbear.com/tr/gumus-rengi-biker-ceket-l03710313?pelement=602084876&categoryId=1030210554#colorId=808").Result;
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
