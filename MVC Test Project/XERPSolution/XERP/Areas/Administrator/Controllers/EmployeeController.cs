using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using XEPR.Models;

namespace XEPR.Areas.Administrator.Controllers
{
    public class EmployeeController : Controller
    {
        List<SelectListItem> lstStateName = new List<SelectListItem>();
        List<SelectListItem> lstCityName = new List<SelectListItem>();
        List<SelectListItem> lstCountryName = new List<SelectListItem>();

        XEPREntities db = new XEPREntities();
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string empId)
        {
            if (!string.IsNullOrEmpty(empId))
            {
                tblEmployee employee = db.tblEmployees.Single(x => x.EmpWinId == empId);

                if (employee != null)
                {
                    ViewBag.EmployeeName = employee.FirstName + " " + employee.LastName;

                    return RedirectToAction("EmployeeHomePage", "Employee");
                }
                else
                {
                    ViewBag.ErrorMessage = "Employee not found.!";
                }

            }
            return View();
        }

        public ActionResult GetEmployeeList()
        {
            List<CustomEmployeeModel> employeeList = new List<CustomEmployeeModel>();

            var employees = (from m in db.tblEmployees
                             join d in db.tblDepartments on m.DepartmentId equals d.DepartmentId
                             select new
                             {
                                 m.EmpWinId,
                                 m.FirstName,
                                 m.LastName,
                                 m.MobileNo,
                                 m.DOB,
                                 m.DOJ,
                                 m.EmailAddress,
                                 m.Address,
                                 m.EmployeeId,
                                 m.Gender,
                                 d.DepartmentName
                             }
                                    );
            foreach (var mvm in employees)
            {
               CustomEmployeeModel m = new CustomEmployeeModel();
                m.EmpWinId = mvm.EmpWinId;
                m.FirstName = mvm.FirstName;
                m.LastName = mvm.LastName;
                m.DOB = mvm.DOB;
                m.DOJ = mvm.DOJ;
                m.EmailAddress = mvm.EmailAddress;
                m.Department = mvm.DepartmentName;
                m.Address = mvm.Address;
                m.EmployeeId = mvm.EmployeeId;
                m.Gender = mvm.Gender;
                employeeList.Add(m);
            }
            return View(employeeList);
        }

        [HttpGet]
        [ActionName("CreateEmployee")]
        public ActionResult CreateEmployee()
        {
            ViewBag.CountryName = BindCountry();   //first request come and move to this method
            ViewBag.StateName = new SelectList(lstStateName, "Value", "Text");
            ViewBag.CityName = new SelectList(lstCityName, "Value", "Text");
            ViewBag.ddlManagers = new SelectList(db.tblManagers, "ManagerWinId", "FirstName");

            populateDropdownlist();
            //ViewBag.ddlDepartments = new SelectList(db.tblDepartments, "DepartmentId", "DepartmentName"); 
            return View();
        }

        [HttpPost]
        [ActionName("CreateEmployee")]
        public ActionResult SaveEmployee(tblEmployee employee)
        {
            int Empcount = (from e in db.tblEmployees where e.EmpWinId == employee.EmpWinId select e.EmpWinId).Count();
            if (Empcount > 0)
            {
                ModelState.AddModelError("EmpWinId", "Win ID Already Exit");
            }
            if (ModelState.IsValid && Session["UserId"].ToString() != null)
            {
                DateTime now = DateTime.Now;
                employee.ManagerWinId = employee.ManagerWinId;
                employee.CreatedDateTime = now;
                employee.ModifiedDateTime = now;
                employee.CreatedBy = Session["UserId"].ToString();
                db.tblEmployees.AddObject(employee);

                db.SaveChanges();
                return RedirectToAction("GetEmployeeList");
            }
            //ViewBag.ddlDepartments = new SelectList(db.tblDepartments, "DepartmentId", "DepartmentName");
            populateDropdownlist();
            ViewBag.CountryName = BindCountry();   //first request come and move to this method
            ViewBag.StateName = new SelectList(lstStateName, "Value", "Text");
            ViewBag.CityName = new SelectList(lstCityName, "Value", "Text");
            ViewBag.ddlManagers = new SelectList(db.tblManagers, "ManagerWinId", "FirstName");
            return View();
        }

        [HttpGet]
        public ActionResult EditEmployee(int id)
        {

            tblEmployee employee = db.tblEmployees.Single(emp => emp.EmployeeId == id);
            ViewBag.CountryName = BindCountry();   //first request come and move to this method
            ViewBag.StateName = new SelectList(db.tblStates, "StateId", "StateName");
            ViewBag.CityName = new SelectList(db.tblCities, "CityId", "CityName");
            ViewBag.ddlDepartments = new SelectList(db.tblDepartments, "DepartmentId", "DepartmentName");
            ViewBag.ddlManagers = new SelectList(db.tblManagers, "ManagerWinId", "FirstName");
            return View(employee);
        }

        [HttpPost]
        public ActionResult EditEmployee(tblEmployee employee)
        {
            if (ModelState.IsValid && Session["UserId"].ToString() != null)
            {
                tblEmployee emp = db.tblEmployees.First(e => e.EmployeeId == employee.EmployeeId);
                emp.FirstName = employee.FirstName;
                emp.LastName = employee.LastName;
                emp.DOB = employee.DOB;
                emp.DOJ = employee.DOJ;
                emp.EmailAddress = employee.EmailAddress;
                emp.Gender = employee.Gender;
                emp.MobileNo = employee.MobileNo;
                emp.Address = employee.Address;
                emp.DepartmentId = employee.DepartmentId;
                emp.ModifiedDateTime = DateTime.Now;
                emp.CityId = employee.CityId;
                emp.StateId = employee.StateId;
                emp.CreatedBy = Session["UserId"].ToString();
                emp.CountryId = employee.CountryId;
                emp.ManagerWinId = employee.ManagerWinId;
                db.SaveChanges();
                return RedirectToAction("GetEmployeeList");
            }
            ViewBag.CountryName = BindCountry();   //first request come and move to this method
            ViewBag.StateName = new SelectList(lstStateName, "Value", "Text");
            ViewBag.CityName = new SelectList(lstCityName, "Value", "Text");
            ViewBag.ddlDepartments = new SelectList(db.tblDepartments, "DepartmentId", "DepartmentName");
            ViewBag.ddlManagers = new SelectList(db.tblManagers, "ManagerWinId", "FirstName");
            return View();
        }

        public ActionResult DeleteEmployee(int id)
        {
            if (ModelState.IsValid)
            {
                tblEmployee employee = db.tblEmployees.Single(emp => emp.EmployeeId == id);
                db.tblEmployees.DeleteObject(employee);
                db.SaveChanges();
            }
            return RedirectToAction("GetEmployeeList");
        }
        public ActionResult EmployeeDetails(int id)
        {
            var emp = from m in db.tblEmployees
                      join d in db.tblDepartments on m.DepartmentId equals d.DepartmentId
                      join c in db.tblCities on m.CityId equals c.CityId
                      join ct in db.tblCountries on m.CountryId equals ct.CountryId
                      join st in db.tblStates on m.StateId equals st.StateId
                      where m.EmployeeId == id
                      select new
                      {
                          m.EmpWinId,
                          m.FirstName,
                          m.LastName,
                          m.MobileNo,
                          m.DOB,
                          m.DOJ,
                          m.EmailAddress,
                          m.Address,
                          m.EmployeeId,
                          m.Gender,
                          c.CityName,
                          ct.CountryName,
                          st.StateName,
                          d.DepartmentName
                      };
            CustomEmployeeModel csemp = new CustomEmployeeModel();
            csemp.EmployeeId = emp.First().EmployeeId;
            csemp.EmpWinId = emp.First().EmpWinId;
            csemp.FirstName = emp.First().FirstName;
            csemp.LastName = emp.First().LastName;
            csemp.DOB = emp.First().DOB;
            csemp.DOJ = emp.First().DOJ;
            csemp.EmailAddress = emp.First().EmailAddress;
            csemp.Address = emp.First().Address;
            csemp.MobileNo = emp.First().MobileNo;
            csemp.Department = emp.First().DepartmentName;
            csemp.Gender = emp.First().Gender;
            csemp.CityName = emp.First().CityName;
            csemp.CountryName = emp.First().CountryName;
            csemp.StateName = emp.First().StateName;

            return View(csemp);
        }

        private void populateDropdownlist()
        {
            List<SelectListItem> selectedListItem = new List<SelectListItem>();
            foreach (tblDepartment department in db.tblDepartments)
            {
                SelectListItem selecteditem = new SelectListItem
                {
                    Text = department.DepartmentName,
                    Value = department.DepartmentId.ToString(),
                    Selected = department.IsSelected
                };

                selectedListItem.Add(selecteditem);
            }

            ViewBag.DepartmentList = selectedListItem;
        }

        public SelectList BindCountry()
        {
            List<tblCountry> lstCountry = new List<tblCountry>();
            lstCountry = db.tblCountries.ToList();

            foreach (var item in lstCountry)
            {
                lstCountryName.Add(new SelectListItem
                {
                    Value = item.CountryId.ToString(),
                    Text = item.CountryName.ToString(),
                });

            }
            return new SelectList(lstCountryName, "Value", "Text");
        }

        public JsonResult BindState(int CountryId)
        {
            List<tblState> lstState = new List<tblState>();
            lstState = db.tblStates.ToList();

            var lstFState = from S in lstState
                            where S.CountryId == CountryId
                            select new { S.StateId, S.StateName };


            foreach (var item in lstFState)
            {
                lstStateName.Add(new SelectListItem
                {
                    Value = item.StateId.ToString(),
                    Text = item.StateName.ToString(),
                });

            }
            return Json(lstStateName, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindCity(int StateId)
        {
            List<tblCity> lstCity = new List<tblCity>();
            lstCity = db.tblCities.ToList();

            var lstFCity = from C in lstCity
                           where C.StateId == StateId
                           select new { C.CityId, C.CityName };


            foreach (var item in lstFCity)
            {
                lstCityName.Add(new SelectListItem
                {
                    Value = item.CityId.ToString(),
                    Text = item.CityName.ToString(),
                });

            }
            return Json(lstCityName, JsonRequestBehavior.AllowGet);
        }

    }
}
