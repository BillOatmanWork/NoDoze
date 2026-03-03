using System;
using System.Threading;
using NoDozer;

namespace NoDozeTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Preventing sleep...");
            using (NoDoze.PreventSleep())
            {
                Console.WriteLine("Sleeping for 5 seconds (system sleep prevented)...");
                Thread.Sleep(60000);
                Console.WriteLine("Done.");
            }
            Console.WriteLine("Sleep allowed again.");
        }
    }
}
