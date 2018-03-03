using System.Web.Mvc;
using EntityFrameworkDemo.Models;
using System.Collections.Generic;
using System.Linq;

namespace EntityFrameworkDemo.Controllers
{
    public class EmployeesController : Controller
    {
        // GET: Employees
        public ActionResult GetEmployees()
        {
            Employee employee = new Employee();
            TestEntities entities = new TestEntities();
            List<Employee> employees = new List<Employee>();
            employees = entities.Employees.ToList();
            return View(employees);
        }
    }
}