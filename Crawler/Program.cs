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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;
using static Crawler.Kategori;
using static Crawler.Program;
using static Crawler.pulladnbeer;

namespace Crawler
{
    internal class Program {
        public static string title;
        public static string discount_price;
        public static async Task Main()
        {
            try
            {
                await Sec();
                //await pull();

                Console.WriteLine("Çıkmak için bir tuşa basın...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                throw;
            }
           
        }
        //public static async Task pull()
        //{
        //    try {
        //        pulladnbeer pulladnbeer = new pulladnbeer();
        //        Console.WriteLine(pulladnbeer);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    } 
        //}

        public static async Task Sec()
        {
            Console.WriteLine("0 | Tüm Kategoriler ve Etkinlik Sayıları ");
            Console.WriteLine();
            Console.WriteLine("1  | Tiyatro ");
            Console.WriteLine();
            Console.WriteLine("2  | Konser ");
            Console.WriteLine();
            Console.WriteLine("3  | Festival ");
            Console.WriteLine();
            Console.WriteLine("6  | Trendler (Tüm Etkinlikler)");
            Console.WriteLine();
            Console.WriteLine("9  | Komedi ");
            Console.WriteLine();
            Console.WriteLine("16 | Çocuk Aktiviteleri ");
            Console.WriteLine();
            Console.WriteLine("40 | Stand-Up ");
            Console.WriteLine();
            
            Console.WriteLine("Kategeri Seç (Başındaki sayı ile seçim yapılmaktadır.)");
            string allKategori = Console.ReadLine();
            if(allKategori == "0")
            { 
                Kategori kategori = new Kategori();
                await kategori.GetKategori();

                Console.WriteLine("Kategeri Seç (Başındaki sayı ile seçim yapılmaktadır.)");
                allKategori = Console.ReadLine();
            }
            if(allKategori == "0") 
            {
                await Sec();
            }
            Console.WriteLine("Lütfen şehrinizin plaka numarasını giriniz.");
            string city = Console.ReadLine();
            if(city == "0")
            {
                Console.WriteLine("GEÇERLİ ŞEHİR NUMARASI GİRİNİZ !!!");
                Thread.Sleep(3000);
                await Sec();
            }
            if(allKategori != "0")
            {
                BuBiletCrawler crawler = new BuBiletCrawler();
                await crawler.GetEventById(allKategori,city);
            }
            else
            {
                Console.WriteLine("GEÇERLİ KATEGORİ NUMARASI GİRİNİZ !!!");
                Thread.Sleep(3000);
                await Sec();
            }
        }
    }
}
