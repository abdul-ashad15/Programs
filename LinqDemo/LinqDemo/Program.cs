using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<string> stringList = new List<string>() {
            "C# Tutorials",
            "VB.NET Tutorials",
            "Learn C++",
            "MVC Tutorials" ,
            "Java"
            };

            //Query Syntax
            var result = from s in stringList
                         where s.Contains("Tutorials")
                         select s;

            //Method Syntax
            // Student collection
            IList<Student> studentList = new List<Student>() {
            new Student() { StudentID = 1, StudentName = "John", Age = 13} ,
            new Student() { StudentID = 2, StudentName = "Moin",  Age = 21 } ,
            new Student() { StudentID = 3, StudentName = "Bill",  Age = 18 } ,
            new Student() { StudentID = 4, StudentName = "Ram" , Age = 20} ,
            new Student() { StudentID = 5, StudentName = "Ron" , Age = 15 }
            };

            List<Student>  teenStudents = studentList.Where(s => s.Age > 12 && s.Age < 20).ToList();
        }
    }

    public class Student
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public int Age { get; set; }
    }
}
