using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Crawler.BuBiletCrawler;
using static Crawler.Kategori;

namespace Crawler
{
    public class Kategori
    {
        public class Dosya
        {
            public int Id { get; set; }
            public string Url { get; set; }
            public string GosterimYeri { get; set; }
            public int Genislik { get; set; }
            public int Yukseklik { get; set; }
            public int Sira { get; set; }
        }

        public class Etiket
        {
            public int Id { get; set; }
            public string Slug { get; set; }
            public string SayfaBaslik { get; set; }
            public string Adi { get; set; }
            public string SeoTanim { get; set; }
            public string Detay { get; set; }
            public string SeoAnahtarKelimeler { get; set; }
            public int Sira { get; set; }
            public bool AnaSayfa { get; set; }
            public List<Dosya> Dosyalar { get; set; }
        }

    
        public async Task GetKategori()
        {
            string apiUrl = "https://apiv2.bubilet.com.tr/api/Etiket";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        List<Etiket> etiket = JsonConvert.DeserializeObject<List<Etiket>>(responseBody);

                        for (int i = 0; i < etiket.Count; i++)
                        {
                            Etiket etiket1 = etiket[i];
                            int etiketSayisi = await KategoriCountEvent(etiket1.Id);
                            if(etiketSayisi > 10)
                            {
                                Console.WriteLine($"{etiket1.Id,-5} | {etiket1.Adi,-20} | {etiketSayisi}");

                                Console.WriteLine();
                            }
                            

                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: " + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                }
            }
    
        }

        public async Task<int> KategoriCountEvent(int etiketId)
        {
            string apiUrl = $"https://apiv2.bubilet.com.tr/api/Anasayfa/{etiketId}/Etkinlikler";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    List<Etkinlik> etkinlikList = JsonConvert.DeserializeObject<List<Etkinlik>>(responseBody);
                    return etkinlikList.Count;
                }
                else
                {
                    throw new Exception($"Etkinlik sayısını alma hatası: {response.StatusCode}");
                }
            }

        }
    }
}
