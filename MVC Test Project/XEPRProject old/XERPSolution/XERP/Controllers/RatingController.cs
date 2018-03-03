using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XEPR.Models;


namespace XEPR.Controllers
{
    public class RatingController : Controller
    {
         [HttpGet]
        public ActionResult CreateRating()
        {
            return View();
        }

        [HttpPost]
         public ActionResult CreateRating(tblRating rating)
         {
             if (ModelState.IsValid)
             {
                 DateTime now = DateTime.Now;
                 XEPREntities db = new XEPREntities();

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
            XEPREntities db = new XEPREntities();
            tblRating ratings = db.tblRatings.Single(x => x.RatingId == id);
            return View(ratings);
        }

        [ActionName("ModifyRating")]
        [HttpPost]
        public ActionResult ModifyRating(tblRating rating)
        {
            if (ModelState.IsValid)
            {
                XEPREntities db = new XEPREntities();

                tblRating rate = db.tblRatings.First(i => i.RatingId == rating.RatingId);
                rate.RatingText = rating.RatingText;
                rate.RatingValue = rating.RatingValue;
                //depart.CreatedBy = "";
                DateTime now = DateTime.Now;
                rate.ModifiedDateTime = now;
                db.SaveChanges();
            }

            return RedirectToAction("GetRatingList");

        }

        public ActionResult GetRating(int id)
        {
            XEPREntities db = new XEPREntities();
            tblRating rating = db.tblRatings.Single(x => x.RatingId == id);
            return View(rating);
        }

        public ActionResult DeleteRating(int id)
        {
            if (ModelState.IsValid)
            {
                XEPREntities db = new XEPREntities();
                tblRating rating = db.tblRatings.Single(x => x.RatingId == id);
                db.tblRatings.DeleteObject(rating);
                db.SaveChanges();
            }
            return RedirectToAction("GetRatingList");
        }

         public ActionResult GetRatingList()
         {
             XEPREntities db = new XEPREntities();

             return View(db.tblRatings.ToList());

             //List<tblRating> ratingList = db.tblRatings.ToList();
             //return View(ratingList);
         }

        

        

    }
}
