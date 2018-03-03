using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NugetDemo;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            MathClass obj = new MathClass();
            Console.Write(obj.Add(1, 2));
            Console.ReadLine();
        }
    }
}
