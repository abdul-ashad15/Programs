using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using XEPR.Models;

namespace XEPR.Areas.Administrator.Controllers
{
    public class AdminController : Controller
    {
        XEPREntities db = new XEPREntities();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult AdminPage()
        {
            ViewBag.Message = "Welcome to Admin Home Page";

            if (!string.IsNullOrEmpty(Session["UserId"].ToString()))
            {
                ViewBag.UserLogin = Session["UserId"].ToString();
            }
            return View();

        }

        [HttpPost]
        public ActionResult Login(tblAdmin admin)
        {
            tblAdmin tAdmin = db.tblAdmins.Where(a => a.UserName == admin.UserName && a.Password == admin.Password).FirstOrDefault();
            if (tAdmin != null)
            {
                Session["UserId"] = admin.UserName;
                return RedirectToAction("AdminPage", "Admin");
            }
            else
            {
                ViewBag.UserLoginError = "User Credential is invalid.";
            }
            return View();
        }

        public ActionResult LogOff()
        {

            //if (Session["UserId"] != null)
            //{
                Session["UserId"] = null;
                Session.Clear();
                Session.Abandon();
                Session.Remove("UserId");
            //}
            return RedirectToAction("Login", "Admin");
        }
    }
}
