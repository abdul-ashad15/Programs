using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace XEPR.Areas.Employee.Models
{
    public class EmployeeAssessment
    {
        public int AssessmentId { get; set; }
        public int RatingId { get; set; }
        public string AssessmentTitle { get; set; }
        public string AssessmentDescription { get; set; }
        public List<SelectListItem> Rating { get; set; }
        public string Comment { get; set; }
    }
}