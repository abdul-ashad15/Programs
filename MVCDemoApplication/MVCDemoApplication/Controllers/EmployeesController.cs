using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCDemoApplication.Models;

namespace MVCDemoApplication.Controllers
{
    public class EmployeesController : Controller
    {
        // GET: Employees
        public ActionResult Index()
        {
            ViewBag.Employees = new List<string>()
            {
                "Ashad",
                "Ashad1",
                "Ashad2",
                "Ashad3"
            };
            return View();
        }

        public ActionResult GetEmployees()
        {
            Employee employee = new Employee()
            {
                EmployeeId = 1001,
                EmployeeName = "Ashad"
            };
            return View(employee);
        }

        public ActionResult Demo()
        {
            ViewBag.Employee = "Employees Controller";
            ViewData["text"] = "Data from view data";
            return View();
        }

        public JsonResult EmployeeJson()
        {
            List<Employee> employees = new List<Employee>
            {
                new Employee { EmployeeId = 1001, EmployeeName = "Ashad"},
                new Employee { EmployeeId = 1002, EmployeeName = "Ashad"},
                new Employee { EmployeeId = 1003, EmployeeName = "Ashad"},
                new Employee { EmployeeId = 1004, EmployeeName = "Ashad"},
                new Employee { EmployeeId = 1005, EmployeeName = "Ashad"},
                new Employee { EmployeeId = 1006, EmployeeName = "Ashad"},
                new Employee { EmployeeId = 1007, EmployeeName = "Ashad"}
            };
            
            return Json(employees, JsonRequestBehavior.AllowGet);
        }
    }
}