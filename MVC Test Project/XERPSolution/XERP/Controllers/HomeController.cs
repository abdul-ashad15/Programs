using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using XEPR.Models;
using XEPR.Common;

namespace XEPR.Controllers
{
    public class HomeController : Controller
    {
        XEPREntities db = new XEPREntities();

        public ActionResult Index()
        {
            ViewBag.Message = "Looking Ahead to 2016";
                       
            return View();
        }

       
    }
}
