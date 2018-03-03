using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHashConcepts
{
    public class Program : Employees
    {
        static void Main(string[] args)
        {
            //Employees employee = new Employees();
            //employee.EmplyoyeeId = 1001; // Public member accessible everywhere
            //Program program = new Program();
            //program.EmplyoyeeId = 1001; // Public member accessible from derived class object
            //program.RollNumber = 15; // Protected member accessible from derived class object

            Employees emplyoee = new Employees();
            emplyoee.EmployeeDivision = "A"; // Protect Internal member accessible from derived class from same assembly
            Console.WriteLine(MyOldCurrencyExchange(500, 1.18));
            Console.WriteLine(MyNewCurrencyExchange(500, 1));
            Console.WriteLine(MyNewCurrencyExchange(500));
            Console.WriteLine(MyOldCurrencyExchange(rate: 1.2, amount: 250)); //Named Parameter
            Console.WriteLine(MyOldCurrencyExchange(amount: 200, rate: 3.2)); //Named Parameter
            Console.ReadLine();
        }

        public static double MyOldCurrencyExchange(double amount, double rate)
        {
            return (amount * rate);
        }

        //Optional Parameter
        public static double MyNewCurrencyExchange(double amount, double rate = 1.18)
        {
            return (amount * rate);
        }
    }

    public class Customer
    {
        public void method1()
        {
            Employees employee = new Employees();
        }
        
    }

    //public class Employees
    //{
    //    public int EmplyoyeeId { get; set; }
    //    private string EmplyoeeName { get; set; }
    //    protected int RollNumber { get; set; }      

    //}
}
