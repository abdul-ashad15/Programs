using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

using XEPR.Areas.Employee.Models;
using XEPR.Models;

namespace XEPR.Areas.Employee.Controllers
{
    public class EmployeeAssessmentController : Controller
    {
        XEPREntities db = new XEPREntities();
        EmployeeAssessment employeementAssessment = new EmployeeAssessment();
        List<EmployeeAssessment> lstEmployeeAssessment = new List<EmployeeAssessment>();
        
        public ActionResult GetAssesmentList()
        {
            string EmpWinId = Convert.ToString(Session["empWinId"]);
            ViewBag.ddlRating = new SelectList(db.tblRatings, "RatingId", "RatingText");
            List<EmployeeAssessment> lstEmployeeAssessment = new List<EmployeeAssessment>();

            if (IsAssessmentSaved(EmpWinId))
            {
                var AssessmentData = (from b in db.tblAssessments
                                      join d in db.tblAssessmentDetails on b.AssessmentId equals d.AssessmentId
                                      where d.EmpWinId == EmpWinId
                                      select new
                                      {
                                          b.AssessmentId,
                                          b.AssessmentTitle,
                                          b.Description,
                                          d.Comment,
                                          d.RatingId


                                      });
                var empstatus = db.tblSubmitAssessments.FirstOrDefault(x => x.EmpWinId == EmpWinId);

                if (empstatus != null)
                {
                    ViewBag.IsEmployeeSubmitted = empstatus.IsEmployeeSubmitted;

                }

                foreach (var item in AssessmentData)
                {

                    EmployeeAssessment employeementAssessment = new EmployeeAssessment();

                    employeementAssessment.AssessmentTitle = item.AssessmentTitle;
                    employeementAssessment.AssessmentDescription = item.Description;
                    employeementAssessment.Comment = item.Comment;

                    employeementAssessment.AssessmentId = item.AssessmentId;
                    employeementAssessment.RatingId = item.RatingId;
                    employeementAssessment.Rating = RatingList(Convert.ToDecimal(item.RatingId));

                    // employeementAssessment.Rating = RatingtList();
                    //tblRating rating = db.tblRatings.Single(x => x.RatingId == item.RatingId);

                    //employeementAssessment.Rating = new SelectList(db.tblRatings, "RatingId", "RatingText", "selected").ToList();
                    lstEmployeeAssessment.Add(employeementAssessment);
                }

            }
            else
            {
                var assessments = (from a in db.tblAssessments
                                   select new
                                   {
                                       a.AssessmentId,
                                       a.AssessmentTitle,
                                       a.Description,
                                   });

                if (assessments.ToList().Count > 0)
                {
                    foreach (var assessment in assessments)
                    {
                        EmployeeAssessment employeementAssessment = new EmployeeAssessment();

                        employeementAssessment.AssessmentTitle = assessment.AssessmentTitle;
                        employeementAssessment.AssessmentDescription = assessment.Description;
                        employeementAssessment.AssessmentId = assessment.AssessmentId;
                        employeementAssessment.Rating = RatingList();
                        lstEmployeeAssessment.Add(employeementAssessment);

                    }
                }
            }
            return View(lstEmployeeAssessment);

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

        [NonAction]
        private List<SelectListItem> RatingList(Decimal selectedValue=0)
        {
            List<SelectListItem> lstSelectedListItem = new List<SelectListItem>();

            foreach (tblRating item in db.tblRatings)
            {
                SelectListItem selectedListItem = new SelectListItem
                {
                    Text = item.RatingValue + "-" + item.RatingText,
                    Value = Convert.ToString(item.RatingId),
                    Selected = (item.RatingValue==selectedValue)? true:item.IsSelected
                };

                lstSelectedListItem.Add(selectedListItem);
            }
            return lstSelectedListItem;
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "Save")]
        public ActionResult SaveAssessment(List<EmployeeAssessment> empAssessment)
        {
            string EmpWinId = Convert.ToString(Session["empWinId"]);

            if (ModelState.IsValid)
            {
                DateTime now = DateTime.Now;

                if (IsAssessmentSaved(EmpWinId) == true)
                {
                    foreach (EmployeeAssessment item in empAssessment)
                    {
                        tblAssessmentDetail assmnt = db.tblAssessmentDetails.FirstOrDefault(x => x.AssessmentId == item.AssessmentId && x.EmpWinId == EmpWinId);

                        if ((assmnt != null && assmnt.AssessmentId == item.AssessmentId))
                        {
                            if (ModelState.IsValid)
                            {
                                tblAssessmentDetail assdet = db.tblAssessmentDetails.First(i => i.AssessmentId == item.AssessmentId && i.EmpWinId == EmpWinId);
                                assdet.RatingId = item.RatingId;
                                assdet.Comment = item.Comment;
                                assdet.ModifiedDateTime = now;

                                db.SaveChanges();
                            }
                        }
                    }
                }
                else
                {

                    foreach (EmployeeAssessment item in empAssessment)
                    {
                        tblAssessmentDetail assdetails = new tblAssessmentDetail();

                        assdetails.AssessmentId = item.AssessmentId;
                        assdetails.RatingId = item.RatingId;
                        assdetails.Comment = item.Comment;
                        assdetails.EmpWinId = Convert.ToString(Session["empWinId"]);// "30118737"; // Employee ID from session
                        assdetails.ManagerWinId = Convert.ToString(Session["managerWinId"]);// "103";// You need to change as ManagerWinID
                        assdetails.CreatedDateTime = now;
                        assdetails.ModifiedDateTime = now;
                        db.tblAssessmentDetails.AddObject(assdetails);

                        db.SaveChanges();
                        assdetails = null;
                    }
                }
            }
            return RedirectToAction("GetAssesmentList");
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "Submit")]

        public ActionResult SubmitAssessment(List<EmployeeAssessment> empAssessment)
        {
            string EmpWinId = Convert.ToString(Session["empWinId"]);

            if (ModelState.IsValid)
            {
                DateTime now = DateTime.Now;


                foreach (EmployeeAssessment item in empAssessment)
                {
                    tblAssessmentDetail assmnt = db.tblAssessmentDetails.FirstOrDefault(x => x.AssessmentId == item.AssessmentId && x.EmpWinId == EmpWinId);

                    if (assmnt != null && assmnt.AssessmentId == item.AssessmentId)
                    {
                        if (ModelState.IsValid)
                        {
                            tblAssessmentDetail assdet = db.tblAssessmentDetails.First(i => i.AssessmentId == item.AssessmentId && i.EmpWinId == EmpWinId);
                            assdet.RatingId = item.RatingId;
                            assdet.Comment = item.Comment;
                            assdet.ModifiedDateTime = now;

                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        tblAssessmentDetail assdetails = new tblAssessmentDetail();

                        assdetails.AssessmentId = item.AssessmentId;
                        assdetails.RatingId = item.RatingId;
                        assdetails.Comment = item.Comment;
                        assdetails.EmpWinId = Convert.ToString(Session["empWinId"]);// "EMP002"; // Employee ID from session
                        assdetails.ManagerWinId = Convert.ToString(Session["managerWinId"]);// "103";
                        assdetails.CreatedDateTime = now;
                        assdetails.ModifiedDateTime = now;
                        db.tblAssessmentDetails.AddObject(assdetails);

                        db.SaveChanges();
                    }

                }
                tblSubmitAssessment subasses = new tblSubmitAssessment();

                subasses.EmpWinId = Convert.ToString(Session["empWinId"]); //"50001626"; // Employee win ID from Session
                subasses.ManagerWinId = Convert.ToString(Session["managerWinId"]);// "P302016"; // Manager WinID for logged in employee
                subasses.IsEmployeeSubmitted = true;
                subasses.IsManagerSubmitted = false;
                subasses.SubmittedDateByEmployee = now;
                subasses.SubmittedDateByManager = now;//
                db.tblSubmitAssessments.AddObject(subasses);
                db.SaveChanges();
            }

            return RedirectToAction("GetAssesmentList");
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
    }

}
