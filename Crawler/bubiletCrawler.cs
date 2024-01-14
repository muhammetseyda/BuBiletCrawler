using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;
using static Crawler.Program;

namespace Crawler
{
    public class BuBiletCrawler
    {
        #region Models
        public class Dosya
        {
            public int id { get; set; }
            public string url { get; set; }
            public string gosterimYeri { get; set; }
            public int genislik { get; set; }
            public int yukseklik { get; set; }
            public int sira { get; set; }
        }

        public class Seans
        {
            public int etkinlikId { get; set; }
            public bool seansGizle { get; set; }
            public int mekanId { get; set; }
            public DateTime tarih { get; set; }
            public int kalanBilet { get; set; }
            public double fiyat { get; set; }
            public double indirimliFiyat { get; set; }
            public int etkinligeKalanGun { get; set; }
            public int satilanBilet { get; set; }
            public int seansId { get; set; }
            public bool koltukSecimi { get; set; }
            public bool yardimEtkinlik { get; set; }
        }

        public class Etkinlik
        {
            public string etkinlikAdi { get; set; }
            public string slug { get; set; }
            public List<Dosya> dosyalar { get; set; }
            public int sira { get; set; }
            public int[] sanatcilar { get; set; }
            public List<int> mekanlar { get; set; }
            public List<Seans> seanslar { get; set; }
            public int etkinlikId { get; set; }
            public bool tumSehirlerdeGoster { get; set; }
            public List<int> seansIdler { get; set; }
            public int toplamKalanBilet { get; set; }
            public int toplamSatilanBilet { get; set; }
            public DateTime etkinlikTarihi { get; set; }
            public int etkinligeKalanGun { get; set; }
        }
        public class Session
        {
            public string mekanAdi { get; set; }
            public int seansId { get; set; }
            public int sehirId { get; set; }
            public DateTime tarih { get; set; }
            public double fiyat { get; set; }
            public double indirimliFiyat { get; set; }
            public int kalanBilet { get; set; }
            public int biletCesitAdet { get; set; }
            public int oturmaPlaniId { get; set; }
            public bool koltukSecimi { get; set; }
            public int sepetimdekiKoltukAdedi { get; set; }
            public bool kombineBilet { get; set; }
            public DateTime? etkinlikBitisTarihi { get; set; }
            public string altBaslik { get; set; }
            public bool koltuksuz { get; set; }
            public bool yardimEtkinlik { get; set; }
        }

        public class EtkinlikData
        {
            public int cityId { get; set; }
            public int count { get; set; }
            public List<Session> sessions { get; set; }
        }

        public class ApiResponse
        {
            public string message { get; set; }
            public bool success { get; set; }
            public List<EtkinlikData> data { get; set; }
            public object errors { get; set; }
        }
        public class Mekan
        {
            public int Id { get; set; }
            public int SehirId { get; set; }
            public string Baslik { get; set; }
            public string KisaBaslik { get; set; }
            public string Adres { get; set; }
            public string Aciklama { get; set; }
            public string Slug { get; set; }
            public int UyePuan { get; set; }
            public int Puan { get; set; }
            public int PuanAdet { get; set; }
            public List<object> Dosyalar { get; set; }
            public string SeoBaslik { get; set; }
            public string SeoAnahtar { get; set; }
            public string SeoAciklama { get; set; }
            public double Enlem { get; set; }
            public double Boylam { get; set; }
        }
        public class Sanatci
        {
            public int Id { get; set; }
            public string Ad { get; set; }
            public string Soyad { get; set; }
            public int SanatciTipi { get; set; }
            public string DogumYeri { get; set; }
            public string Biografi { get; set; }
            public List<Dosya> Dosyalar { get; set; }
            public string Slug { get; set; }
            public int EtkinlikAdedi { get; set; }
            public string SeoBaslik { get; set; }
            public string SeoAnahtar { get; set; }
            public string SeoAciklama { get; set; }
            public string AdiSoyadi { get; set; }
        }
        #endregion
        
        public async Task GetEventById(string id, string cityId)
        {
            string apiUrl = $"https://apiv2.bubilet.com.tr/api/Anasayfa/{id}/Etkinlikler"; // 1 tiyatro --

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        List<Etkinlik> etkinlikDataList = JsonConvert.DeserializeObject<List<Etkinlik>>(responseBody);
                        
                        Console.WriteLine("Veriler alınıyor...");
                        

                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        FileInfo excelFile = new FileInfo("BubiletVerileri.xlsx");
                        using (ExcelPackage package = new ExcelPackage(excelFile))
                        {
                            ExcelWorksheet existingWorksheet = package.Workbook.Worksheets.FirstOrDefault(sheet => sheet.Name == $"Bubilet{id}");
                            if (existingWorksheet != null)
                            {
                                package.Workbook.Worksheets.Delete(existingWorksheet);
                            }
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add($"Bubilet{id}");

                            worksheet.Cells[1, 2].Value = "Bilet Sitesi";
                            worksheet.Cells[1, 3].Value = "Etkinlik Id";
                            worksheet.Cells[1, 4].Value = "Etkinlik Adı";
                            worksheet.Cells[1, 5].Value = "Sanatçı";
                            worksheet.Cells[1, 6].Value = "Mekan Adı";
                            worksheet.Cells[1, 7].Value = "Tarih";
                            worksheet.Cells[1, 8].Value = "Fiyat";
                            worksheet.Cells[1, 9].Value = "Mekan Adresi";
                            worksheet.Cells[1, 1].Value = "Link";
                            worksheet.Cells[1, 1, 1, 9].Style.Font.Bold = true;
                            worksheet.Cells[1, 1, 1, 9].Style.Font.Size = 14;
                            worksheet.Cells[1, 1, 1, 9].AutoFilter = true;


                            worksheet.Column(1).Width = 10;
                            worksheet.Column(2).Width = 13;
                            worksheet.Column(3).Width = 13;
                            worksheet.Column(4).Width = 40;
                            worksheet.Column(5).Width = 30;
                            worksheet.Column(7).Style.Numberformat.Format = "dd.MM.yyyy HH:mm";
                            worksheet.Column(6).Width = 35;
                            worksheet.Column(7).Width = 20;
                            worksheet.Column(8).Width = 10;
                            worksheet.Column(9).Width = 50;


                            int rowIndex = 2;
                            int totalEvents = etkinlikDataList.Count;

                            for (int i = 0; i < etkinlikDataList.Count; i++)
                            {
                                Etkinlik etkinlikData = etkinlikDataList[i];

                                var sanatci = await Sanatcigetir(etkinlikData.sanatcilar);
                                string sanatciNames = string.Join(", ", sanatci);

                                foreach (var seans in etkinlikData.seanslar)
                                {
                                    var mekan = await MekanGet(seans.mekanId);
                                    if (mekan.SehirId.ToString() == cityId || etkinlikData.tumSehirlerdeGoster == true)
                                    {
                                        City city = new City();
                                        var cityName = await city.ReadCity(mekan.SehirId);

                                        worksheet.Cells[rowIndex, 2].Value = "bubilet.com";
                                        worksheet.Cells[rowIndex, 3].Value = etkinlikData.etkinlikId;
                                        worksheet.Cells[rowIndex, 4].Value = etkinlikData.etkinlikAdi;
                                        worksheet.Cells[rowIndex, 5].Value = sanatciNames;
                                        worksheet.Cells[rowIndex, 6].Value = mekan.Baslik;
                                        worksheet.Cells[rowIndex, 7].Value = seans.tarih;
                                        worksheet.Cells[rowIndex, 8].Value = seans.indirimliFiyat;
                                        worksheet.Cells[rowIndex, 9].Value = mekan.Adres;
                                        var hyperlink = new ExcelHyperLink($"https://www.bubilet.com.tr/{cityName.ToLower()}/etkinlik/{etkinlikData.slug}");
                                        worksheet.Cells[rowIndex, 1].Hyperlink = hyperlink;
                                        worksheet.Cells[rowIndex, 1].Style.Font.UnderLine = true;

                                        worksheet.Cells[rowIndex, 1].Value = "Tıklayınız";

                                        rowIndex++;
                                    }
                                }
                                int currentProgress = (i + 1) * 100 / totalEvents;
                                ProgressBar.UpdateLoading(currentProgress, 100);
                            }
                            package.Save();
                        }
                        Console.WriteLine();
                        Console.WriteLine("Excel dosyası oluşturuldu.");
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

        public async Task GetCityId(string etkinlikId)
        {
            string apiUrl = "https://apiv2.bubilet.com.tr/api/Etkinlik/" + etkinlikId + "/sessions/all";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // JSON verisini nesne olarak al
                        ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);

                        foreach (var etkinlikData in apiResponse.data)
                        {
                            if (etkinlikData.cityId == 34)
                            {
                                foreach (var session in etkinlikData.sessions)
                                {
                                    string mekanAdı = session.mekanAdi;
                                    Console.WriteLine($"      City ID: {etkinlikData.cityId} - Mekan Adı: {mekanAdı}");
                                }
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

        public async Task<Mekan> MekanGet(int mekanId)
        {
            string apiUrl = "https://apiv2.bubilet.com.tr/api/Mekan/" + mekanId;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        Mekan mekan = JsonConvert.DeserializeObject<Mekan>(responseBody);
                        
                        return mekan;
                    }
                    else
                    {
                        Console.WriteLine("Error: " + response.StatusCode);
                        throw new Exception($"Failed Mekan. Status Code: {response.StatusCode}");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);

                    throw new Exception($"Exception Mekan: {ex.Message}");
                }
            }
        }

        public async Task<string[]> Sanatcigetir(int[] sanatciIds)
        {
            List<string> ad = new List<string>();

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    foreach (int sanatciId in sanatciIds)
                    {
                        string apiUrl = "https://apiv2.bubilet.com.tr/api/Sanatci/" + sanatciId;

                        HttpResponseMessage response = await client.GetAsync(apiUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();

                            // JSON verisini nesne olarak al
                            Sanatci sanatci = JsonConvert.DeserializeObject<Sanatci>(responseBody);

                            ad.Add(sanatci.Ad + " " + sanatci.Soyad);
                        }
                        else
                        {
                            Console.WriteLine($"Error for artist ID {sanatciId}: {response.StatusCode}");
                            ad.Add(response.StatusCode.ToString());
                        }
                    }

                    return ad.ToArray();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                    return new string[] { ex.Message };
                }
            }
        }
    }
}
