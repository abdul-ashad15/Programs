using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace XEPR.Models
{
    [MetadataType(typeof(RatingMetadata))]
    public partial class tblRating
    {
    }

    public class RatingMetadata
    {
        [DisplayAttribute(Name = "Rating Text")]
        [Required(ErrorMessage = "Please enter RatingText.")]
        public string RatingText { get; set; }

        [DisplayAttribute(Name = "Rating Value")]
        [Required(ErrorMessage = "Please enter Rating Value.")]
        [RegularExpression("^[0-9]*$",ErrorMessage = "Rating Value must be a Number.")]
        
        public string RatingValue { get; set; }

        [DisplayAttribute(Name = "Created By")]
        [Required(ErrorMessage = "Please enter Created By.")]

        public string CreatedBy { get; set; }
    }
}