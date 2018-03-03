using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymousTypeDemo
{
    class Program
    {
        public delegate int DelAdd(int x, int y);
        static void Main(string[] args)
        {
            Program prog = new Program();
            var item = new { id=1001,name ="Ashad"};
            List<Employee> employees = new List<Employee>()
            {
                new Employee {Id= 1001, Ename = "Abdul Ashad1"},
                new Employee {Id= 1002, Ename = "Abdul Ashad2"},
                new Employee {Id= 1003, Ename = "Abdul Ashad3"},
                new Employee {Id= 1004, Ename = "Abdul Ashad4"},
                new Employee {Id= 1005, Ename = "Abdul Ashad5"},
            };

            Employee employee = employees.Find(delegate(Employee emp)
            {
                return emp.Id == 1002;
            });

            DelAdd obj = delegate (int a, int b)
            {
                return a + b;
            };

            DelAdd o
            //int result = obj.Invoke(2, 3);
        }

        //public static int Add(int x,int y)
        //{
        //    return x + y;
        //}
    }

    public class Employee
    {
        public int Id { get; set; }
        public string Ename { get; set; }
    }
}
