using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Immutable
{
    class Program
    {
        public const int value = 5;
        public readonly int rvalue = 10;

        public Program()
        {
            rvalue = 12;
        }
        static void Main(string[] args)
        {
            string strMyValue = "Hello Visitor";
            Program obj = new Program();
            TestClass testClass = new TestClass();
            // create a new string instance instead of changing the old one
            strMyValue += "How Are";
            strMyValue += "You ??";
            Console.WriteLine(strMyValue);
            Console.WriteLine(obj.GetType());
            StringBuilder s = new StringBuilder();
            Console.WriteLine(s.GetType());
            Console.ReadLine();
        }
    }

    public class TestClass
    {

    }
}
