using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace XEPR.Areas.Employee.Models
{
    public class ManagerReviewForEmpolyee
    {
        public int AssessmentId { get; set; }
        public string AssessmentTitle { get; set; }
        public string ManagerWinId { get; set; }
        public string EmpWinId { get; set; }
        public int Manager_RatingId { get; set; }
        public List<SelectListItem> ManagerRating { get; set; }
        public string Manager_Comment { get; set; }
        public string AssessmentDescription { get; set; }
        
        [DisplayAttribute(Name = " Employee First Name")]
        public string FirstName { get; set; }
      

    }
}