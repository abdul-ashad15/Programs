using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncawaitDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //Normal Call
            //Method1();
            //Method2();
            AsyncMethod1();
            AsyncMethod2();
            Console.ReadKey();
        }

        public static void Method1()
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(" Method 1");
            }
        }

        public static void Method2()
        {
            for (int i = 0; i < 25; i++)
            {
                Console.WriteLine(" Method 2");
            }
        }

        public static async Task AsyncMethod1()
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Console.WriteLine(" Method 1");
                }
            });
        }

        public static void AsyncMethod2()
        {
            for (int i = 0; i < 25; i++)
            {
                Console.WriteLine(" Method 2");
            }
        }
    }
}
