using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace XEPR.Models
{
     [MetadataType(typeof(AssessmentMetadata))]
    public partial class tblAssessment
    {
    }
     public class AssessmentMetadata
     {
         [DisplayAttribute(Name = "Assessment Title")]
         [Required(ErrorMessage = "Please enter Assessment Title.")]
         public string AssessmentTitle { get; set; }

         //[DisplayAttribute(Name = "CreatedBy")]
         //[Required(ErrorMessage = "Please enter Created By.")]
         //public string CreatedBy { get; set; }
     }

}