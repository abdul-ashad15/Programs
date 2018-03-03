using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDemo
{
    class Program
    {
        public enum Days
        {
            sunday = 10,
            monday,
            tuesday,
            wednesday,
            friday,
            saturday
        };
        static void Main(string[] args)
        {
            int i = 4;
            int j = i;
            i = 10;
            Test test1 = new Test(1, "Ashad");
            Console.WriteLine("Test ID = {0} Name = {1}", test1.ID, test1.Name);

            Test test2 = new Test(2, "AshadNew");
            test2 = test1;

            test2.ID = 2;
            test2.Name = "Ashad2";

            Console.WriteLine("Test ID = {0} Name = {1}", test2.ID, test2.Name);
            Console.WriteLine("Test ID = {0} Name = {1}", test1.ID, test1.Name);
            Console.ReadLine();

            string[] values = Enum.GetNames(typeof(Days));
            int day = (int)Days.sunday;
            string daystring = Days.sunday.ToString();
        }
    }

    public class Test
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public Test(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}
