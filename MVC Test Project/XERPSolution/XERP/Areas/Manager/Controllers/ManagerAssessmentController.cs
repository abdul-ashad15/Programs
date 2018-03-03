using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;

using XEPR.Areas.Manager.Models;
using XEPR.Models;

namespace XEPR.Areas.Manager.Controllers
{
    public class ManagerAssessmentController : Controller
    {
        XEPREntities db = new XEPREntities();
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "Save")]
        public ActionResult SaveAssessment(List<ManagerAssessment> lstManagerAssessment)
        {
            if (ModelState.IsValid)
            {
                DateTime now = DateTime.Now;
                tblAssessmentDetail assdetails = new tblAssessmentDetail();

                string empWinID = Request.Form["EmpWinId"].ToString();

                if (IsAssessmentSaved(empWinID))
                {
                    foreach (ManagerAssessment item in lstManagerAssessment)
                    {
                        tblAssessmentDetail assmnt = db.tblAssessmentDetails.Single(x => x.AssessmentId == item.AssessmentId && x.EmpWinId == empWinID);
                        if (assmnt.AssessmentId == item.AssessmentId)
                        {
                            if (ModelState.IsValid)
                            {
                                tblAssessmentDetail assdet = db.tblAssessmentDetails.First(i => i.AssessmentId == item.AssessmentId && i.EmpWinId == empWinID);
                                assdet.Manager_RatingId = item.Manager_RatingId;
                                assdet.Manager_Comment = item.Manager_Comment;
                                assdet.ModifiedDateTime = now;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                else
                {
                    foreach (ManagerAssessment item in lstManagerAssessment)
                    {
                        assdetails.AssessmentId = item.AssessmentId;
                        assdetails.Manager_RatingId = item.Manager_RatingId;
                        assdetails.Manager_Comment = item.Manager_Comment;
                        assdetails.EmpWinId = Request.Form["EmpWinId"];// "EMP002"; // Employee Win ID
                        assdetails.ManagerWinId = Convert.ToString(Session["managerWinId"]); //103;
                        assdetails.CreatedDateTime = now;
                        assdetails.ModifiedDateTime = now;
                        db.tblAssessmentDetails.AddObject(assdetails);
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("DisplayEmployeeAssementDetails", "ManagerAssessment", new { EmpWinId = Request.Form["EmpWinId"] });
        }

        [HttpGet]
        public ActionResult DisplayEmployeeAssementDetails(string EmpWinId)
        {
            List<ManagerAssessment> lstEmployeeAssessment = new List<ManagerAssessment>();
            String mgrWinId;
            mgrWinId = Convert.ToString(Session["managerWinId"]);
            var mgrstatus = db.tblSubmitAssessments.FirstOrDefault(x => x.EmpWinId == EmpWinId);
            if (mgrstatus != null)
            {
                ViewBag.IsManagerSubmitted = mgrstatus.IsManagerSubmitted;
            }

            ViewData["IsEnabled"] = false;

            //string EmpWinId = "123";
            var assessments = from m in db.tblAssessmentDetails
                              join d in db.tblAssessments on m.AssessmentId equals d.AssessmentId
                              where m.EmpWinId == EmpWinId
                              select new
                              {
                                  m.AssessmentId,
                                  d.AssessmentTitle,
                                  d.Description,
                                  m.Comment,
                                  m.Manager_Comment
                              };
            if (assessments.ToList().Count > 0)
            {
                foreach (var assessment in assessments)
                {
                    ManagerAssessment ManagerAssessment = new ManagerAssessment();
                    ManagerAssessment.AssessmentTitle = assessment.AssessmentTitle;
                    ManagerAssessment.AssessmentDescription = assessment.Description;
                    ManagerAssessment.AssessmentId = assessment.AssessmentId;
                    ManagerAssessment.Comment = assessment.Comment;
                    ManagerAssessment.Manager_Comment = assessment.Manager_Comment;
                    ManagerAssessment.Rating = RatingList();
                    ManagerAssessment.ManagerRating = RatingList();
                    lstEmployeeAssessment.Add(ManagerAssessment);
                }
            }
            return View(lstEmployeeAssessment);
        }

        [HttpGet]
        public ActionResult GetAssesmentList()
        {
            string mid = Convert.ToString(Session["managerWinId"]);
            List<CustomEmployeeModel> employeeList = new List<CustomEmployeeModel>();
            var employees = (from m in db.tblEmployees
                             join d in db.tblSubmitAssessments on m.EmpWinId equals d.EmpWinId
                             into EmpSubmit
                             from EmpAssessment in EmpSubmit.DefaultIfEmpty()
                             where m.ManagerWinId == mid
                             select new
                             {
                                 EmpWinId = m.EmpWinId,
                                 FirstName = m.FirstName,
                                 LastName = m.LastName,
                                 EmployeeId = m.EmployeeId,
                                 IsEmployeeSubmitted = (EmpAssessment.IsEmployeeSubmitted == null) ? false : EmpAssessment.IsEmployeeSubmitted
                             });

            foreach (var mvm in employees)
            {
                CustomEmployeeModel m = new CustomEmployeeModel();
                m.EmpWinId = mvm.EmpWinId;
                m.FirstName = mvm.FirstName;
                m.LastName = mvm.LastName;
                m.EmployeeId = mvm.EmployeeId;
                m.IsEmployeeSubmitted = mvm.IsEmployeeSubmitted;
                employeeList.Add(m);
            }
            return View(employeeList);
        }

        [NonAction]
        private List<SelectListItem> RatingList()
        {
            List<SelectListItem> lstSelectedListItem = new List<SelectListItem>();
            foreach (tblRating item in db.tblRatings)
            {
                SelectListItem selectedListItem = new SelectListItem
                {
                    Text = item.RatingValue + "-" + item.RatingText,
                    Value = Convert.ToString(item.RatingId),
                    Selected = item.IsSelected
                };
                lstSelectedListItem.Add(selectedListItem);
            }
            return lstSelectedListItem;
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "Submit")]

        public ActionResult SubmitAssessment(List<ManagerAssessment> managerAssessment)
        {
            string EmpWinId = Request.Form["EmpWinId"];
            if (ModelState.IsValid)
            {
                DateTime now = DateTime.Now;
                foreach (ManagerAssessment item in managerAssessment)
                {

                    tblAssessmentDetail assmnt = db.tblAssessmentDetails.FirstOrDefault(x => x.AssessmentId == item.AssessmentId && x.EmpWinId == EmpWinId);
                    if (assmnt != null && assmnt.AssessmentId == item.AssessmentId)
                    {
                        if (ModelState.IsValid)
                        {
                            tblAssessmentDetail assdet = db.tblAssessmentDetails.First(i => i.AssessmentId == item.AssessmentId && i.EmpWinId == EmpWinId);
                            assdet.Manager_RatingId = item.Manager_RatingId;
                            assdet.Manager_Comment = item.Manager_Comment;
                            assdet.ModifiedDateTime = now;
                            db.SaveChanges();
                            tblSubmitAssessment subasses = db.tblSubmitAssessments.First(i => i.EmpWinId == EmpWinId);
                            subasses.ManagerWinId = Convert.ToString(Session["managerWinId"]); //"P302016"; // Manager Win ID from session
                            subasses.IsManagerSubmitted = true;
                            subasses.SubmittedDateByEmployee = subasses.SubmittedDateByEmployee;
                            subasses.SubmittedDateByManager = now;
                            //db.tblSubmitAssessments.AddObject(subasses);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        tblAssessmentDetail assdetails = new tblAssessmentDetail();
                        assdetails.AssessmentId = item.AssessmentId;
                        assdetails.Manager_RatingId = item.Manager_RatingId;
                        assdetails.Manager_Comment = item.Manager_Comment;
                        assdetails.EmpWinId = Request.Form["EmpWinId"];// "EMP002"; // Employee Win ID
                        assdetails.ManagerWinId = Convert.ToString(Session["managerWinId"]);// 103;  // Manager Win ID from session
                        assdetails.CreatedDateTime = now;
                        assdetails.ModifiedDateTime = now;
                        db.tblAssessmentDetails.AddObject(assdetails);
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("DisplayEmployeeAssementDetails", "ManagerAssessment", new { EmpWinId = Request.Form["EmpWinId"] });

        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class MultiButtonAttribute : ActionNameSelectorAttribute
        {
            public string MatchFormKey { get; set; }
            public string MatchFormValue { get; set; }
            public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
            {
                return controllerContext.HttpContext.Request[MatchFormKey] != null &&
                    controllerContext.HttpContext.Request[MatchFormKey] == MatchFormValue;
            }
        }

        [NonAction]
        private bool IsAssessmentSaved(string EmpWinId)
        {
            tblAssessmentDetail tmpid = db.tblAssessmentDetails.FirstOrDefault(x => x.EmpWinId == EmpWinId);

            if (tmpid != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
