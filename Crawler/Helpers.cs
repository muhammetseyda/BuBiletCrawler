using System;

namespace Crawler
{
    internal class Helpers
    {
        public static double Price(string price)
        {
            price = price.Replace(" TL", "");
            price = price.Replace(".", ",");
            return Convert.ToDouble(price);
        }
        public static double Price2(string price)
        {
            price = price.Replace("₺", "");
            price = price.Replace(".", ",");
            return Convert.ToDouble(price);
        }
    }
}