using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Crawler.BuBiletCrawler;

namespace Crawler
{
    public class City
    {
        public class CityEvent
        {
            public int SehirKod { get; set; }
            public int etkinlikAdedi { get; set; }
        }

        public async Task GetCityEvent(int cityId)
        {
            string apiUrl = "https://apiv2.bubilet.com.tr/api/etkinlik/EtkinlikSehirleri";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        List<CityEvent> cityevent = JsonConvert.DeserializeObject<List<CityEvent>>(responseBody);
                        foreach (var item in cityevent)
                        {
                            var cityName = ReadCity(item.SehirKod);
                            Console.OutputEncoding = Encoding.UTF8;
                            Console.WriteLine(item.SehirKod + " : " + cityName.Result  + " : " + item.etkinlikAdedi);

                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: " + response.StatusCode);
                        throw new Exception($"Failed CityEvent. Status Code: {response.StatusCode}");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);

                    throw new Exception($"Exception CityEvent: {ex.Message}");
                }
            }

        }

        public async Task<string> ReadCity(int cityId)
        {
            try
            {
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "City.json");
                string jsonContent = File.ReadAllText(jsonPath, Encoding.UTF8);

                Dictionary<int, string> cityDictionary = JsonConvert.DeserializeObject<Dictionary<int, string>>(jsonContent);
                return cityDictionary[cityId];

            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);

                throw new Exception($"Exception ReadCity: {ex.Message}");
            }
        }
    }
}
