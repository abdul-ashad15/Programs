using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethodOverLoadingDemo
{
    class Program
    {
        void print(int i)
        {
            Console.WriteLine("Printing int: {0}", i);
        }

        void print(double f)
        {
            Console.WriteLine("Printing float: {0}", f);
        }

        void print(string s)
        {
            Console.WriteLine("Printing string: {0}", s);
        }

        public int Add(int x, int y)
        {
            return x + y;
        }

        public double  Add(double x, double y)
        {
            return x + y;
        }
        static void Main(string[] args)
        {
            Program p = new Program();

            // Call print to print integer
            p.print(5);

            // Call print to print float
            p.print(500.263);

            // Call print to print string
            p.print("Hello C++");
            Console.ReadKey();
        }
    }
}
