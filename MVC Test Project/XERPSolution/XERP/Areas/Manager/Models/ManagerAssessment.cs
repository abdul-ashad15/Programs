using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace XEPR.Areas.Manager.Models
{
    public class ManagerAssessment
    {
        public int AssessmentId { get; set; }
        public int RatingId { get; set; }
        public string AssessmentTitle { get; set; }
        public string AssessmentDescription { get; set; }
        public List<SelectListItem> Rating { get; set; }
        public string Comment { get; set; }

        public string ManagerWinId { get; set; }
        public string EmpWinId { get; set; }


        public int Manager_RatingId { get; set; }
        public List<SelectListItem> ManagerRating { get; set; }
        public string Manager_Comment { get; set; }

        //  public int EmpIoyeeId { get; set; }
        [DisplayAttribute(Name = " Employee First Name")]
        public string FirstName { get; set; }
        //public int MangerId { get; set; }
    }

    public class EmployeeListAssessment
    {

        public List<SelectListItem> EmpIoyeeId { get; set; }
        public string Comment { get; set; }

    }
}