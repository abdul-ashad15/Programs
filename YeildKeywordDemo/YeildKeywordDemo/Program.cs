using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YeildKeywordDemo
{
    class Program
    {
        //Yield : Yield keyword is used to do stateful custom iteration over a collection
        static List<int> values = new List<int>();
        static void Main(string[] args)
        {
            FillValues();

            foreach(int i in Filter())
            {
                Console.WriteLine(i);
            }
            Console.ReadLine();
        }

        public static IEnumerable<int> Filter()
        {
            foreach(int i in values)
            {
                if (i > 3)
                    yield return i;
            }
        }

        public static void FillValues()
        {
            values.Add(1);
            values.Add(2);
            values.Add(3);
            values.Add(4);
            values.Add(5);
            values.Add(6);
        }
    }
}
