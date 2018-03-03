using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XEPR.Models;

using PagedList;
using PagedList.Mvc;

using XEPR.Common;

namespace XEPR.Areas.Administrator.Controllers
{
    public class DepartmentController : Controller
    {
        XEPREntities db = new XEPREntities();

        [HttpGet]
        public ActionResult CreateDepartment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateDepartment(tblDepartment department)
        {
            if (ModelState.IsValid && Session["UserId"].ToString() != null)
            {
                try
                {
                    DateTime now = DateTime.Now;

                    department.CreatedBy = Session["UserId"].ToString();
                    department.IsSelected = false;
                    department.CreatedDateTime = now;
                    department.ModifiedDateTime = now;
                    db.tblDepartments.AddObject(department);

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Unable to save changes", ex);
                }
            }
            return RedirectToAction("GetDepartmentList");
        }

        [HttpGet]
        [ActionName("ModifyDepartment")]
        public ActionResult ModifyDepartment(int id)
        {
            tblDepartment departments = db.tblDepartments.Single(x => x.DepartmentId == id);
            return View(departments);
        }

        [ActionName("ModifyDepartment")]
        [HttpPost]
        public ActionResult ModifyDepartment(tblDepartment department)
        {
            if (ModelState.IsValid && Session["UserId"].ToString() != null)
            {
                tblDepartment depart = db.tblDepartments.First(i => i.DepartmentId == department.DepartmentId);
                depart.DepartmentName = department.DepartmentName;
                depart.Description = department.Description;
                depart.CreatedBy = Session["UserId"].ToString();
                DateTime now = DateTime.Now;
                depart.ModifiedDateTime = now;
                db.SaveChanges();
            }

            return RedirectToAction("GetDepartmentList");
        }

        public ActionResult GetDepartment(int id)
        {
            tblDepartment departments = db.tblDepartments.Single(x => x.DepartmentId == id);
            return View(departments);
        }

        public ActionResult DeleteDepartment(int id)
        {
            if (ModelState.IsValid)
            {
                tblDepartment departments = db.tblDepartments.Single(x => x.DepartmentId == id);
                db.tblDepartments.DeleteObject(departments);
                db.SaveChanges();
            }
            return RedirectToAction("GetDepartmentList");
        }

        [OutputCache(Duration=20)]
        //[OutputCache(CacheProfile = "OneMinute")]
        //[PartialCache("OneMinute")] // it is used for Child action
        public ActionResult GetDepartmentList(int ? page)
        {
            XEPREntities db = new XEPREntities();

           return View(db.tblDepartments.ToList().ToPagedList(page ?? 1, 3));

            //string search = "E";

            //return View(db.tblDepartments.Where(d => d.DepartmentName.StartsWith(search) || search == null ).ToList().ToPagedList(page ?? 1, 3));

            //List<tblDepartment> departmentList = db.tblDepartments.ToList();//.ToPagedList(page ?? 1,3);
            //return View(departmentList);
        }

    }
}
