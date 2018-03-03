using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceDemo
{
    public interface IEmployee
    {
        //int x; // Can't contain members

        //public int MyProperty { get; set; } // Can't contain properties
        string Name(); //By default public
    }
    public sealed class SealedClass 
    {
        public int MyProperty { get; set; }
    }

    public abstract class AbstractClass // Abstract class can contain Abstract and Non Abstract Method
    {
        int x;
        public abstract int Mul(int x, int y); // Abstract Method
        
        public int Sub(int x, int y) // Non Abstract Method
        {
            return x - y;
        }
    }

    public class BaseClass //Normal class can't contain Abstract method
    {
        //public abstract string GetName(); //Normal class can't contain Abstract method

        public int Add(int x, int y)
        {
            return x + y;
        }
    }

    public class DerivedClass : BaseClass
    {
        public int Add(int x, int y)
        {
            return x + y;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            BaseClass baseClass = new BaseClass();
            Console.WriteLine("Calling Base method" + baseClass.Add(3, 4));

            DerivedClass derivedClass = new DerivedClass();
            Console.WriteLine("Calling Derived method" + derivedClass.Add(10, 10));

            BaseClass obj = new DerivedClass();
            Console.WriteLine("Calling Base method" + obj.Add(11, 11));

            // Not possible
            //DerivedClass derivedObject = new BaseClass();
        }
    }
}
