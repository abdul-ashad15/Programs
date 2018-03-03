using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonDemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string jSon = "{\"FirstName\":\"Ayesha\",\"LastName\":\"Rafique\"}";
            var employee = JsonConvert.DeserializeObject<Employee>(jSon);
            Console.WriteLine("Employee FirstName :" + employee.FirstName + " "+"Employee LastName:" + " " + employee.LastName);
            Console.ReadLine();
        }
    }

    public class Employee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
