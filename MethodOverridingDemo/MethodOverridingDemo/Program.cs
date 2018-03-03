using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethodOverridingDemo
{
    public class Bclass
    {
        public virtual void Sample1()
        {
            Console.WriteLine("Base Class");
        }
    }
    // Derived Class
    public class DClass : Bclass
    {
        public override void Sample1()
        {
            Console.WriteLine("Derived Class");
        }
    }
    // Using base and derived class
    class Program
    {
        static void Main(string[] args)
        {
            // calling the overriden method
            DClass objDc = new DClass();
            objDc.Sample1();
            
            // calling the base class method
            Bclass objBc = new Bclass();
            objBc.Sample1();

            // calling the derived class method
            Bclass ob = new DClass();
            ob.Sample1();

            // Error
            //DClass dobj = new Bclass();

            Console.ReadLine();
        }
    }
}
