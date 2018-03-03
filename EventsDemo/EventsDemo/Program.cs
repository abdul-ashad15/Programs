using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsDemo
{
    public delegate void MyEventHandler();
    class Program
    {
        public static event MyEventHandler myEvent;
        static void Main(string[] args)
        {
            myEvent += new MyEventHandler(Dog);
            myEvent += new MyEventHandler(Cat);

            myEvent.Invoke();
        }

        public static void Cat()
        {
            Console.WriteLine("Calling Cat Method");
            Console.ReadLine();
        }

        public static void Dog()
        {
            Console.WriteLine("Calling Dog Methos");
        }
    }
}
