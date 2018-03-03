using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessModifierDemo
{

    public class AccessTest
    {
        public int i = 10;
        private int j = 15;
        protected int k = 20;
        internal int l = 25;

        public void TestMethod()
        {
            AccessTest obj = new AccessTest();
            int a, b, c, d;
            //Within a class if you create an object of same class then you can access all data members through 
            //object reference even private data too
            
            a = obj.i; // Public 
            b = obj.j; //Private
            c = obj.k; //Protected
            d = obj.l; //Internal
        }
    }
    class Program
    {
        //static void Main(string[] args)
        //{
        //    AccessTest accessTest = new AccessTest();
            
        //    //In different class if we create an object of parent class then only public members are accessible
        //    int pub = accessTest.i; // Public
        //    int inter = accessTest.l;
        //}
    }

    public class BaseClass
    {
        public BaseClass()
        {
            Console.WriteLine("Base Class Constructor executed");
        }

        public void Write()
        {
            Console.WriteLine("Write method in Base Class executed");
        }
    }

    public class ChildClass : BaseClass
    {
        public ChildClass()
        {
            Console.WriteLine("Child Class Constructor executed");
        }

        public static void Main()
        {
            ChildClass CC = new ChildClass();
            CC.Write();
            Console.ReadLine();
        }
    }

    /*Output
    Base Class Constructor executed
    Child Class Constructor executed
    Write method in Base Class executed */

    public class Demo : AccessTest
    {

        public void TestMethod()
        {
            Demo obj = new Demo();

            //Protected member are accessible from the derived class object
            int a, b;
            a = obj.i; //Public
            b = obj.k; //Protected
            int c = obj.l; // Internal
        }
    }
}
