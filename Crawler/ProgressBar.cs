using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler
{
    public class ProgressBar
    {
        public static void Loading(int length)
        {
            int progressBarLength = length;

            for (int i = 0; i <= progressBarLength; i++)
            {
                Console.Write("[");
                for (int j = 0; j < progressBarLength; j++)
                {
                    if (j < i)
                    {
                        Console.Write("#");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.Write("] {0}% ", i * 100 / progressBarLength);

                Thread.Sleep(100);

                Console.SetCursorPosition(0, Console.CursorTop - 1);
            }

            Console.WriteLine("\nİşlem tamamlandı.");
            Console.ReadLine();
        }
        public static void UpdateLoading(int currentProgress, int totalProgress)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("[");
            for (int j = 0; j < totalProgress; j++)
            {
                if (j < currentProgress)
                {
                    Console.Write("#");
                }
                else
                {
                    Console.Write(" ");
                }
            }
            Console.Write("] {0}% ", currentProgress * 100 / totalProgress);
        }
    }
}
