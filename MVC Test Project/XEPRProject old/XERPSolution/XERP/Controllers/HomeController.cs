using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XEPR.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Looking Ahead to 2016";
            return View();
        }

        public ActionResult AdminPage()
        {
            ViewBag.Message = "Welcome to Admin Home Page";
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
    }
}
