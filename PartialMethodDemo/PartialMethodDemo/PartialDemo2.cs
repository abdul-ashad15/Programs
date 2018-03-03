using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialMethodDemo
{
    public partial class PartialDemo1
    {
        partial void Method1()
        {
            Console.WriteLine("Partial Method");
        }

        public static void Main()
        {
            PartialDemo1 obj = new PartialDemo1();
            obj.Method1();
        }
    }
}
