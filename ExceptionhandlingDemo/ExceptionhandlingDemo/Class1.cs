using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionhandlingDemo
{
    public class Class1 : IComparable,IComparer
    {
        public static void Main()
        {
            //List<string> myList = new List<string>();
            //for (int i = 0; i < 10; i++)
            //{
            //    myList.Add("Item " + i.ToString());
            //}

            //IEnumerator<string> myEnum = myList.GetEnumerator();

            //myEnum.Reset();
            //myEnum.MoveNext();
            //myEnum.MoveNext();
            //myEnum.MoveNext();
            //System.Console.WriteLine(myEnum.Current);
            //myEnum.MoveNext();
            //myEnum.MoveNext();
            //System.Console.WriteLine(myEnum.Current);
            //myEnum.Reset();
            //myEnum.MoveNext();
            //System.Console.WriteLine(myEnum.Current);
            Class1 obj1 = new Class1();
            Class1 obj2 = new Class1();
            int count = obj1.Compare(obj1, obj2);



        }

        public int Compare(object x, object y)
        {
            if (x == y)
                return 1;
            else
                return 0;
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
