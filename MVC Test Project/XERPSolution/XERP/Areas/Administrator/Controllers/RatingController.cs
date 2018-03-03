using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XEPR.Models;

namespace XEPR.Areas.Administrator.Controllers
{
    public class RatingController : Controller
    {
        XEPREntities db = new XEPREntities();

        [HttpGet]
        public ActionResult CreateRating()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateRating(tblRating rating)
        {
            if (ModelState.IsValid && Session["UserId"].ToString() != null)
            {
                DateTime now = DateTime.Now;
                rating.CreatedBy = Session["UserId"].ToString();
                rating.IsSelected = false;
                rating.CreatedDateTime = now;
                rating.ModifiedDateTime = now;
                db.tblRatings.AddObject(rating);

                db.SaveChanges();
            }
            return RedirectToAction("GetRatingList");
        }

        [HttpGet]
        [ActionName("ModifyRating")]
        public ActionResult ModifyRating(int id)
        {
            tblRating ratings = db.tblRatings.Single(x => x.RatingId == id);
            return View(ratings);
        }

        [ActionName("ModifyRating")]
        [HttpPost]
        public ActionResult ModifyRating(tblRating rating )
        {
            if (ModelState.IsValid && Session["UserId"].ToString() != null)
            {
                tblRating rate = db.tblRatings.First(i => i.RatingId == rating.RatingId);
                rate.RatingText = rating.RatingText;
                rate.RatingValue = rating.RatingValue;
                rate.Description = rating.Description;
                rate.CreatedBy = Session["UserId"].ToString();
                DateTime now = DateTime.Now;
                rate.ModifiedDateTime = now;
                db.SaveChanges();
            }

            return RedirectToAction("GetRatingList");
        }

        public ActionResult GetRating(int id)
        {
            tblRating rating = db.tblRatings.Single(x => x.RatingId == id);
            return View(rating);
        }

        public ActionResult DeleteRating(int id)
        {
            if (ModelState.IsValid)
            {
                tblRating rating = db.tblRatings.Single(x => x.RatingId == id);
                db.tblRatings.DeleteObject(rating);
                db.SaveChanges();
            }
            return RedirectToAction("GetRatingList");
        }

        public ActionResult GetRatingList()
        {
            return View(db.tblRatings.ToList());
        }

    }
}
