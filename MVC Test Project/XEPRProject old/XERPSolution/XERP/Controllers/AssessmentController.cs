using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XEPR.Models;

namespace XEPR.Controllers
{
    public class AssessmentController : Controller
    {

        [HttpGet]
        public ActionResult CreateAssessment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateAssessment(tblAssessment assessment)
        {
            if (ModelState.IsValid)
            {
                DateTime now = DateTime.Now;
                XEPREntities db = new XEPREntities();


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
            XEPREntities db = new XEPREntities();
            tblAssessment departments = db.tblAssessments.Single(x => x.AssessmentId == id);
            return View(departments);
        }

        [ActionName("ModifyAssessment")]
        [HttpPost]
        public ActionResult ModifyAssessment(tblAssessment assessment)
        {
            if (ModelState.IsValid)
            {
                XEPREntities db = new XEPREntities();

                tblAssessment assat = db.tblAssessments.First(i => i.AssessmentId == assessment.AssessmentId);
                assat.AssessmentTitle = assessment.AssessmentTitle;
                assat.Description = assessment.Description;
                assat.CreatedBy = assessment.CreatedBy;

                //depart.CreatedBy = "";
                DateTime now = DateTime.Now;
                assat.ModifiedDateTime = now;
                db.SaveChanges();
            }

            return RedirectToAction("GetAssessmentList");
        }

        public ActionResult GetAssessment(int id)
        {
            XEPREntities db = new XEPREntities();
            tblAssessment assessments = db.tblAssessments.Single(x => x.AssessmentId == id);
            return View(assessments);
        }

        public ActionResult DeleteAssessment(int id)
        {
            if (ModelState.IsValid)
            {
                XEPREntities db = new XEPREntities();
                tblAssessment assessments = db.tblAssessments.Single(x => x.AssessmentId == id);
                db.tblAssessments.DeleteObject(assessments);
                db.SaveChanges();
            }
            return RedirectToAction("GetAssessmentList");
        }

        public ActionResult GetAssessmentList()
        {
            XEPREntities db = new XEPREntities();
            return View(db.tblAssessments.ToList());
          
        }

    }
}
