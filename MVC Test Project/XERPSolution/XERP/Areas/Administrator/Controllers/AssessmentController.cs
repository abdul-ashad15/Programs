using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XEPR.Models;

namespace XEPR.Areas.Administrator.Controllers
{
    public class AssessmentController : Controller
    {
        XEPREntities db = new XEPREntities();

        [HttpGet]
        public ActionResult CreateAssessment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateAssessment(tblAssessment assessment)
        {
            if (ModelState.IsValid && Session["UserId"].ToString() != null)
            {
                DateTime now = DateTime.Now;

                assessment.CreatedBy = Session["UserId"].ToString();
                assessment.CreatedDateTime = now;
                assessment.ModifiedDateTime = now;
                db.tblAssessments.AddObject(assessment);

                db.SaveChanges();
            }
            return RedirectToAction("GetAssessmentList");
        }
        [HttpGet]
        [ActionName("ModifyAssessment")]
        public ActionResult ModifyAssessment(int id)
        {
            tblAssessment departments = db.tblAssessments.Single(x => x.AssessmentId == id);
            return View(departments);
        }

        [ActionName("ModifyAssessment")]
        [HttpPost]
        public ActionResult ModifyAssessment(tblAssessment assessment)
        {
            if (ModelState.IsValid && Session["UserId"].ToString() != null)
            {
                tblAssessment assat = db.tblAssessments.First(i => i.AssessmentId == assessment.AssessmentId);
                assat.AssessmentTitle = assessment.AssessmentTitle;
                assat.Description = assessment.Description;
                assat.CreatedBy = Session["UserId"].ToString();
                DateTime now = DateTime.Now;
                assat.ModifiedDateTime = now;
                db.SaveChanges();
            }

            return RedirectToAction("GetAssessmentList");
        }

        public ActionResult GetAssessment(int id)
        {
            tblAssessment assessments = db.tblAssessments.Single(x => x.AssessmentId == id);
            return View(assessments);
        }

        public ActionResult DeleteAssessment(int id)
        {
            if (ModelState.IsValid)
            {
                tblAssessment assessments = db.tblAssessments.Single(x => x.AssessmentId == id);
                db.tblAssessments.DeleteObject(assessments);
                db.SaveChanges();
            }
            return RedirectToAction("GetAssessmentList");
        }

        public ActionResult GetAssessmentList()
        {
            return View(db.tblAssessments.ToList());
        }
    }
}
