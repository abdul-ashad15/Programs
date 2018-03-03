using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqDemoNew
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> values = new List<int>();
            values.Add(1);
            values.Add(5);
            values.Add(7);
            values.Add(8);
            values.Add(4);
            values.Add(7);
            values.Add(89);
            values.Add(18);
            values.Add(4);

           IEnumerable<int>  queryValues = values.Where(s => s > 5);

           // IQueryable<int> query = values.Where(s => s >= 7);

            //var queryValues = values.Where(s => s > 5);
        }
    }
}
