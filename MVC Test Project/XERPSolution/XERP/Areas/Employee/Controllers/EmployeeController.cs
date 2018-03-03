using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using XEPR.Models;

namespace XEPR.Areas.Employee.Controllers
{
    public class EmployeeController : Controller
    {

        XEPREntities db = new XEPREntities();

        [HttpGet]
        public ActionResult EmpLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EmpLogin(XEPR.Models.tblEmployee emp)
        {
            if (!string.IsNullOrEmpty(emp.EmpWinId))
            {
                XEPR.Models.tblEmployee employee = db.tblEmployees.FirstOrDefault(x => x.EmpWinId == emp.EmpWinId);

                if (employee != null)
                {
                    Session["empWinId"] = employee.EmpWinId;
                    Session["managerWinId"] = employee.ManagerWinId;
                    Session["EmlopyeeLogin"] = employee.FirstName + " " + employee.LastName;
                    ViewBag.EmployeeName = employee.FirstName + " " + employee.LastName;
                    return RedirectToAction("GetAssesmentList", "EmployeeAssessment");
                }
                else
                {
                    ModelState.AddModelError("EmpWinId", "Employee not found.!");
                }
            }
            return View();
        }

        public ActionResult LogOff()
        {

            if (Session["EmlopyeeLogin"] != null)
            {
                Session["EmlopyeeLogin"] = null;
                Session.Clear();
                Session.Abandon();
                Session.Remove("EmlopyeeLogin");
            }
            return RedirectToAction("EmpLogin", "Employee");
        }

    }
}
