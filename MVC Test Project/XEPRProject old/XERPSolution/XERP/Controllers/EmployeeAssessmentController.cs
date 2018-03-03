using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using XEPR.Models;

namespace XEPR.Controllers
{
    public class EmployeeAssessmentController : Controller
    {
        XEPREntities db = new XEPREntities();

        public ActionResult SaveAssessment(List<EmployeeAssessment> lstEmployeeAssessment)
        {

            return View(lstEmployeeAssessment);
        }

        [HttpGet]
        public ActionResult GetAssesmentList()
        {
            List<EmployeeAssessment> lstEmployeeAssessment = new List<EmployeeAssessment>();

            string winId = "";// this value come from login id

            if (IsAssessmentSaved(winId))
            {
                // if already assessment saved..

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
                        //employeementAssessment.Rating =
                        lstEmployeeAssessment.Add(employeementAssessment);
                    }
                }
            }
            return View(lstEmployeeAssessment);

        }

        private bool IsAssessmentSaved(string winId)
        {
            bool retValue = false;

            // write logic to validate assessment detail for employee

            return retValue;
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
        private ActionResult RatingList(List<EmployeeAssessment> empAssessment)
        {
            return View();
        }

    }
}
