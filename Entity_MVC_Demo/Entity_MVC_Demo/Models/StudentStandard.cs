using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Entity_MVC_Demo.Models
{
    public class Student
    {
        public Student()
        {

        }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public byte[] Photo { get; set; }
        public decimal Height { get; set; }
        public float Weight { get; set; }

        public Standard Standard { get; set; }
    }

    public class Standard
    {
        public Standard()
        {

        }
        public int StandardId { get; set; }
        public string StandardName { get; set; }

        public ICollection<Student> Students { get; set; }
    }
}