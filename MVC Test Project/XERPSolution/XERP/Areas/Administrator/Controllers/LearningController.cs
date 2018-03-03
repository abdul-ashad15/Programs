using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using XEPR.Models;

namespace XEPR.Areas.Administrator.Controllers
{
    public class LearningController : Controller
    {
        XEPREntities db = new XEPREntities();

        List<SelectListItem> lstStateName = new List<SelectListItem>();
        List<SelectListItem> lstCityName = new List<SelectListItem>();
        List<SelectListItem> lstCountryName = new List<SelectListItem>();


        public ActionResult Index()
        {
            ViewBag.Country = BindCountry();   //first request come and move to this method
            ViewBag.State = lstStateName;
            ViewBag.City = lstCityName;
            return View();
        }

        [HttpGet]
        public ActionResult Save()
        {
            ViewBag.Country = BindCountry();   //first request come and move to this method
            ViewBag.State = lstStateName;
            ViewBag.City = lstCityName;

            return View();
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
            return new SelectList(lstCountryName, "Value", "Text", "id");
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

        [HttpGet]
        public ActionResult CountryList()
        {
            var lst = db.tblCountries.ToList();

            List<CountryMetadata> lstContr = new List<CountryMetadata>();

            foreach (var l in lst)
            {
                CountryMetadata cm = new CountryMetadata();

                cm.CountryId = l.CountryId;
                cm.CountryName = l.CountryName;
                lstContr.Add(cm);
            }

            return View(lstContr);
        }

        [HttpPost]
        public ActionResult CountryList(List<CountryMetadata> countries)
        {

            foreach(CountryMetadata conutry in countries)
            {

            }

            return View(countries);
        }

    }

    public class CountryMetadata
    {

        public int CountryId { get; set; }
        public string CountryName { get; set; }

    }
}

