using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymousDemo
{
    //Delegate Initialize
    public delegate int MyDelegate(int x, int y);
    class Program
    {
        static void Main(string[] args)
        {
            //Anonymous Type
            var anonymousType = new {
                ID = 1,
                Name = "Ashad"
            };

            //Delegate object
            MyDelegate delObj = new MyDelegate(Add);
            //Delegate Call
            delObj.Invoke(4,5);

            //Anonymous Method

            MyDelegate delObjAn = delegate (int x, int y)
            {
                return x + y;
            };

            delObjAn.Invoke(2, 3);
        }

        public static int Add(int x, int y)
        {
            return x + y;
        }
    }
}
