using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEnumerableDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> Month = new List<string>();
            Month.Add("January");
            Month.Add("February");
            Month.Add("March");
            Month.Add("April");
            Month.Add("May");
            Month.Add("June");
            Month.Add("July");
            Month.Add("August");
            Month.Add("September");
            Month.Add("October");
            Month.Add("November");
            Month.Add("December");

            IEnumerable<string> months = (IEnumerable<string>)Month;

            foreach(string allMonths in months)
            {
                Console.WriteLine(allMonths);
            }


            //Create IEnumerator of string.

            IEnumerator<string> iEnumeratorOfString = Month.GetEnumerator();//to convert list into IEnumerator we can invoke the GetEnumerator method  

            //To retrieve all the items from the above IEnumerator object, we cannot use foreach loop instead of that we need to invoke MoveNext() Boolean method.  
            while (iEnumeratorOfString.MoveNext())
            {
                Console.WriteLine(iEnumeratorOfString.Current);
            }
            Console.ReadLine();
        }
    }
}
