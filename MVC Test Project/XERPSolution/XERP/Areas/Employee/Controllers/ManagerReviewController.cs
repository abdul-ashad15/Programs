using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XEPR.Areas.Employee.Models;
using XEPR.Models;

namespace XEPR.Areas.Employee.Controllers
{
    public class ManagerReviewController : Controller
    {
        XEPREntities db = new XEPREntities();
        ManagerReviewForEmpolyee managerReviewAssessment = new ManagerReviewForEmpolyee();
        List<ManagerReviewForEmpolyee> lstmgrassessment = new List<ManagerReviewForEmpolyee>();


        public ActionResult GetManagerReviewForEmployee()
        {
           

            string EmpWinId = Convert.ToString(Session["empWinId"]);

            if (IsAssessmentSaved(EmpWinId))
            {
                tblSubmitAssessment mgrdetails = db.tblSubmitAssessments.FirstOrDefault(x => x.EmpWinId == EmpWinId);

                var mgrsubmmit= mgrdetails.IsManagerSubmitted;

                if (mgrdetails != null && mgrsubmmit== true)
                {
                    var AssessmentData = (from b in db.tblAssessments
                                          join d in db.tblAssessmentDetails on b.AssessmentId equals d.AssessmentId
                                          where d.EmpWinId == EmpWinId
                                          select new
                                          {
                                              b.AssessmentId,
                                              b.AssessmentTitle,
                                              b.Description,
                                              d.Manager_Comment,
                                              d.Manager_RatingId,
                                              d.ManagerWinId


                                          });

                    foreach (var item in AssessmentData)
                    {

                        ManagerReviewForEmpolyee mgrreviewAssessment = new ManagerReviewForEmpolyee();
                        mgrreviewAssessment.AssessmentId = item.AssessmentId;
                        mgrreviewAssessment.AssessmentTitle = item.AssessmentTitle;
                        mgrreviewAssessment.AssessmentDescription = item.Description;
                        mgrreviewAssessment.Manager_Comment = item.Manager_Comment;


                       // mgrreviewAssessment.Manager_RatingId = Convert.ToInt16(item.Manager_RatingId);
                        mgrreviewAssessment.ManagerRating = RatingList();
                        lstmgrassessment.Add(mgrreviewAssessment);
                    }


                
                }



            
            }

            return View(lstmgrassessment);
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

    }
}
