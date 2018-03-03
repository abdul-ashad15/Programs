using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace GenericsDemo
{
    public class Check<T>
    {
        public bool Compare(T x, T y)
        {
            if (x.Equals(y))
                return true;
            else
                return false;
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            Check<int> obj = new Check<int>();
            bool b1 = obj.Compare(1, 2);

            Check<string> objString = new Check<string>();
            bool b2 = objString.Compare("ASHAD", "AYESHA");
        }
    }
}
