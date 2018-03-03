using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using XEPR.Models;

namespace XEPR.Areas.Manager.Controllers
{
    public class ManagerController : Controller
    {
        XEPREntities db = new XEPREntities();

        [HttpGet]
        public ActionResult ManagerLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ManagerLogin(tblManager m)
        {
            if (!string.IsNullOrEmpty(m.ManagerWinId))
            {
                tblManager mgr = db.tblManagers.FirstOrDefault(x => x.ManagerWinId == m.ManagerWinId);

                if (mgr != null)
                {
                    Session["managerWinId"] = m.ManagerWinId;
                    Session["ManagerLogin"] = mgr.FirstName + " " + mgr.LastName;
                    ViewBag.EmployeeName = mgr.FirstName + " " + mgr.LastName;

                    return RedirectToAction("GetAssesmentList", "ManagerAssessment");
                }
                else
                {
                    ModelState.AddModelError("ManagerWinId", "Manager not found.!");
                }
            }
            return View();
        }

        public ActionResult LogOff()
        {

            if (Session["ManagerLogin"] != null)
            {
                Session["ManagerLogin"] = null;
                Session.Clear();
                Session.Abandon();
                Session.Remove("ManagerLogin");
            }
            return RedirectToAction("ManagerLogin", "Manager");
        }

    }
}
