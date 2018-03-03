using Entity_MVC_Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Entity_MVC_Demo.Controllers
{
    public class SchoolController : Controller
    {
        // GET: School
        public ActionResult AddStudent()
        {
            using (SchoolContext schoolcontext = new SchoolContext())
            {
                Student stud = new Student() { StudentId = 1001,StudentName = "Ashad New" };

                schoolcontext.Students.Add(stud);
                schoolcontext.SaveChanges();
            }
            return View();
        }
    }
}