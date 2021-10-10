using System;

namespace targil0
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome8220();
            Welcome1626();
            Console.ReadKey();
        }

        static partial void Welcome1626();
        private static void Welcome8220()
        {
            Console.WriteLine("Enter your name");
            string name = Console.ReadLine();
            Console.WriteLine("{0}, welcome to my first console application");
        }
    }
}
