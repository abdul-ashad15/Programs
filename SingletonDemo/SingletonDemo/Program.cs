using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingletonDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Singleton s1 = Singleton.Instance();
            Singleton s2 = Singleton.Instance();

            // Test for same instance
            if (s1 == s2)
            {
                Console.WriteLine("Objects are the same instance");
                Console.ReadLine();
            }
        }
    }

    public class Singleton
    {
        private static Singleton _instance;

        private Singleton()
        {

        }

        public static Singleton Instance()
        {
            if (_instance == null)
            {
                _instance = new Singleton();
            }

            return _instance;
        }
    }
}
