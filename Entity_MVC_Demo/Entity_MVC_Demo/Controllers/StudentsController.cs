using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity_MVC_Demo.Models;

namespace Entity_MVC_Demo.Controllers
{
    public class StudentsController : Controller
    {
        // GET: Students
        public ActionResult GetStudents()
        {
            List<Students> students = new List<Students>();
            using (StudentsContext entities = new StudentsContext())
            {
                students = entities.students.ToList();
            }
            return View(students);
        }

        public ActionResult GetStudentsByID(int id)
        {
            Students students = new Students();
            using (StudentsContext entities = new StudentsContext())
            {
                var student = entities.students.Where(stud => stud.ID == id);
                students = student.FirstOrDefault<Students>();
            }
            return View(students);
        }
    }
}