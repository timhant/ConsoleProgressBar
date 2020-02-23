using Hant.Helper;
using System;
using System.Threading;

namespace ConsoleProgress
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("console progress bar test..");

            // 单线程单任务
            var pBar = new ConsoleProgressBar("s1").Show();

            for (int i = 0; i <= 100; i++)
            {
                Thread.Sleep(50);
                pBar.UpdateProgress(i);
            }

            // 单线程多个任务
            var pBars = new ConsoleProgressBar[9];

            for (int i = 0; i < pBars.Length; i++)
            {
                pBars[i] = new ConsoleProgressBar($"task{i + 1}").Show();
            }

            for (int i = 0; i <= 100; i++)
            {
                Thread.Sleep(100);

                for (int j = 0; j < pBars.Length; j++)
                {
                    pBars[j].UpdateProgress(i);
                }
            }

            // 多线程多任务
            // 有问题 orz
            for (int i = 0; i < 9; i++)
            {
                var thread = new Thread(new ThreadStart(delegate ()
                {
                    var threadName = Thread.CurrentThread.Name;
                    var pBar = new ConsoleProgressBar(threadName).Show();

                    for (int i = 0; i <= 100; i++)
                    {
                        Thread.Sleep(100);
                        pBar.UpdateProgress(i);
                    }
                }));

                thread.Name = $"Thread{i + 1}";
                thread.Start();
            }

            // wait for exit
            Console.ReadKey();
        }
    }
}