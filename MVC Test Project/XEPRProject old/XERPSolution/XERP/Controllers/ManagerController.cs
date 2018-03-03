using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XEPR.Models;

namespace XEPR.Controllers
{
    public class ManagerController : Controller
    {
        XEPREntities db = new XEPREntities();
        List<SelectListItem> lstStateName = new List<SelectListItem>();
        List<SelectListItem> lstCityName = new List<SelectListItem>();
        List<SelectListItem> lstCountryName = new List<SelectListItem>();
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

        [HttpGet]
        public ActionResult CreateManager()
        {           
            populateDropdownlist();
            ViewBag.Country = BindCountry();   //first request come and move to this method
            ViewBag.State = lstStateName;
            ViewBag.City = lstCityName;           
            return View();
        }

        [HttpGet]
        [ActionName("Login")]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        public ActionResult Login(tblManager m)
        {
            if (!string.IsNullOrEmpty(m.ManagerWinId))
            {
                tblManager mgr = db.tblManagers.FirstOrDefault(x => x.ManagerWinId == m.ManagerWinId);

                if (mgr != null)
                {
                    ViewBag.EmployeeName = mgr.FirstName + " " + mgr.LastName;

                    return RedirectToAction("GetManagerList", "Manager");
                }
                else
                {
                    ModelState.AddModelError("ManagerWinId", "Manager not found.!");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateManager(tblManager Manager)
        {
            ViewBag.Country = BindCountry();   //first request come and move to this method
            ViewBag.State = lstStateName;
            ViewBag.City = lstCityName;
            populateDropdownlist();           
            if (ModelState.IsValid)
            {
                if (IsManagerExists(Manager.ManagerWinId) == false)
                {
                    DateTime now = DateTime.Now;                    
                    Manager.CreatedBy = "Admin";
                    Manager.CreatedDateTime = now;
                    Manager.ModifiedDateTime = now;
                    db.tblManagers.AddObject(Manager);
                    db.SaveChanges();
                    return RedirectToAction("GetManagerList");
                }
                else
                {                  
                    ModelState.AddModelError("", "Manager Win Id is already exists.");                   
                }                
            }           
            return View();

        }

        [HttpGet]
        [ActionName("ModifyManager")]
        public ActionResult ModifyManager(int id)
        {
            ViewBag.Country = BindCountry();  //first request come and move to this method
            ViewBag.State = new SelectList(db.tblStates, "StateId", "StateName");
            ViewBag.City = new SelectList(db.tblCities, "CityId", "CityName");
            
            populateDropdownlist();
            List<SelectListItem> genderlist =new List<SelectListItem>();
            genderlist.Add(new SelectListItem { Text = "Male", Value = "Male" });
            genderlist.Add(new SelectListItem { Text = "Female", Value = "Female" });
            ViewBag.GenderList = genderlist;
 
            tblManager Managers = db.tblManagers.Single(x => x.ManagerId == id);
            return View(Managers);
        }

        [ActionName("ModifyManager")]
        [HttpPost]
        public ActionResult ModifyManager(tblManager Manager)
        { 
            if (ModelState.IsValid)
            {
                tblManager Manag = db.tblManagers.First(i => i.ManagerId == Manager.ManagerId);                
                Manag.FirstName = Manager.FirstName;
                Manag.LastName = Manager.LastName;
                Manag.DOB = Manager.DOB;
                Manag.DOJ = Manager.DOJ;               
                Manag.DepartmentId = Manager.DepartmentId;
                Manag.Gender = Manager.Gender;
                Manag.Address = Manager.Address;
                Manag.EmailAddress = Manager.EmailAddress;
                DateTime now = DateTime.Now;
                Manag.ModifiedDateTime = now;
                Manag.CountryId = Manager.CountryId;
                Manag.StateId = Manager.StateId;
                Manag.CityId = Manager.CityId;
                db.SaveChanges(); 
                return RedirectToAction("GetManagerList");
            }
            populateDropdownlist();
            ViewBag.Country = BindCountry();   //first request come and move to this method
            ViewBag.State = lstStateName;
            ViewBag.City = lstCityName;
            List<SelectListItem> genderlist = new List<SelectListItem>();
            genderlist.Add(new SelectListItem { Text = "Male", Value = "Male" });
            genderlist.Add(new SelectListItem { Text = "Female", Value = "Female" });
            ViewBag.GenderList = genderlist;
            return View();
        }

        public ActionResult GetManager(int id)
        {  
            //var ManagerDetail = (from m in db.tblManagers
            //                         join d in db.tblDepartments on m.DepartmentId equals d.DepartmentId
            //                         where m.ManagerId==id
            //                         select new
            //                         {
            //                             m.ManagerId,
            //                             m.FirstName,
            //                             m.LastName,
            //                             m.ManagerWinId,
            //                             m.DOB,
            //                             m.DOJ,
            //                             m.EmailAddress,
            //                             m.Gender,
            //                             d.DepartmentName,                                    
            //                             m.Address,
            //                             m.CreatedBy,
            //                             m.MobileNo
            //                         }
            //                       );

           

  var ManagerDetail = from m in db.tblManagers
                      join d in db.tblDepartments on m.DepartmentId equals d.DepartmentId
                      join c in db.tblCities on m.CityId equals c.CityId
                      join ct in db.tblCountries on m.CountryId equals ct.CountryId
                      join st in db.tblStates on m.StateId equals st.StateId
                      where m.ManagerId == id
                      select new
                     {
                         m.ManagerId,
                         m.FirstName,
                         m.LastName,
                         m.ManagerWinId,
                         m.DOB,
                         m.DOJ,
                         m.EmailAddress,
                         m.Gender,
                         d.DepartmentName,                                    
                         m.Address,
                         m.CreatedBy,
                         m.MobileNo,
                         c.CityName,
                         ct.CountryName,
                         st.StateName                        
                     };        
            CustomManagerView mm = new CustomManagerView();
            mm.ManagerId = ManagerDetail.First().ManagerId;
            mm.FirstName = ManagerDetail.First().FirstName;
            mm.LastName = ManagerDetail.First().LastName;
            mm.ManagerWinId = ManagerDetail.First().ManagerWinId;
            mm.DOB = ManagerDetail.First().DOB;
            mm.DOJ = ManagerDetail.First().DOJ;
            mm.Address = ManagerDetail.First().Address;
            mm.CreatedBy = ManagerDetail.First().CreatedBy;
            mm.EmailAddress = ManagerDetail.First().EmailAddress;
            mm.MobileNo = ManagerDetail.First().MobileNo;
            mm.Gender = ManagerDetail.First().Gender;
            mm.Department = ManagerDetail.First().DepartmentName;
            mm.CountryId = ManagerDetail.First().CountryName;
            mm.StateId = ManagerDetail.First().StateName;
            mm.CityId = ManagerDetail.First().CityName;

            return View(mm);
        }

        public ActionResult DeleteManager(int id)
        {
            if (ModelState.IsValid)
            {
                XEPREntities db = new XEPREntities();
                tblManager Managers = db.tblManagers.Single(x => x.ManagerId == id);
                db.tblManagers.DeleteObject(Managers);
                db.SaveChanges();
            }
            return RedirectToAction("GetManagerList");
        }


        public ActionResult GetManagerList()
        {          
            List<CustomManagerView> managerList = new List<CustomManagerView>();            
            var ManagerDetailList = (from m in db.tblManagers
                                     join d in db.tblDepartments on m.DepartmentId equals d.DepartmentId
                                     select new
                                     {
                                         m.ManagerId,
                                         m.FirstName,
                                         m.LastName,
                                         m.ManagerWinId,
                                         m.DOB,
                                         m.DOJ,
                                         m.EmailAddress,
                                         m.Gender,
                                         d.DepartmentName
                                     }
                                    );          

            foreach (var mvm in ManagerDetailList)
            {
                CustomManagerView m = new CustomManagerView();
                m.ManagerId = mvm.ManagerId;
                m.FirstName = mvm.FirstName;
                m.LastName = mvm.LastName;
                m.ManagerWinId = mvm.ManagerWinId;
                m.DOB = mvm.DOB;
                m.DOJ = mvm.DOJ;
                m.EmailAddress = mvm.EmailAddress;
                m.Gender = mvm.Gender;
                m.Department = mvm.DepartmentName;

                managerList.Add(m);
            }
            return View(managerList);

        }

        [NonAction]
        private bool IsManagerExists(string winID)
        {
            bool retVal = false;
            XEPREntities db = new XEPREntities();
            if (db.tblManagers.Where(e => e.ManagerWinId.Equals(winID)).Count() > 0)
            {
                retVal = true;
            }
            return retVal;           
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


    }
}
