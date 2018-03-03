using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XEPR.Models;

namespace XEPR.Controllers
{
    public class DepartmentController : Controller
    {
        [HttpGet]
        public ActionResult CreateDepartment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateDepartment(tblDepartment department)
        {
            if (ModelState.IsValid)
            {
                DateTime now = DateTime.Now;
                XEPREntities db = new XEPREntities();

                department.IsSelected = false;
                department.CreatedDateTime = now;
                department.ModifiedDateTime = now;
                db.tblDepartments.AddObject(department);

                db.SaveChanges();
            }
            return RedirectToAction("GetDepartmentList");
        }

        [HttpGet]
        [ActionName("ModifyDepartment")]
        public ActionResult ModifyDepartment(int id)
        {
            XEPREntities db = new XEPREntities();
            tblDepartment departments = db.tblDepartments.Single(x => x.DepartmentId == id);
            return View(departments);
        }

        [ActionName("ModifyDepartment")]
        [HttpPost]
        public ActionResult ModifyDepartment(tblDepartment department)
        {
            if (ModelState.IsValid)
            {
                XEPREntities db = new XEPREntities();
                
                tblDepartment depart = db.tblDepartments.First(i => i.DepartmentId == department.DepartmentId);
                depart.DepartmentName = department.DepartmentName;
                depart.Description = department.Description;
                //depart.CreatedBy = "";
                DateTime now = DateTime.Now;
                depart.ModifiedDateTime = now;
                db.SaveChanges();
            }

            return RedirectToAction("GetDepartmentList");
        }

        public ActionResult GetDepartment(int id)
        {
            XEPREntities db = new XEPREntities();
            tblDepartment departments = db.tblDepartments.Single(x => x.DepartmentId == id);
            return View(departments);
        }

        public ActionResult DeleteDepartment(int id)
        {
            if (ModelState.IsValid)
            {
                XEPREntities db = new XEPREntities();
                tblDepartment departments = db.tblDepartments.Single(x => x.DepartmentId == id);
                db.tblDepartments.DeleteObject(departments);
                db.SaveChanges();
            }
            return RedirectToAction("GetDepartmentList");
        }

        public ActionResult GetDepartmentList()
        {
            XEPREntities db = new XEPREntities();
            List<tblDepartment> departmentList = db.tblDepartments.ToList();
            return View(departmentList);
        }

    }
}
