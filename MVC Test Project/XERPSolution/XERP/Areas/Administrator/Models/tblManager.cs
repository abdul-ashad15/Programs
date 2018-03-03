using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace XEPR.Models
{
    [MetadataType(typeof(ManagerMetadata))]
    public partial class tblManager
    {

    }
    public class ManagerMetadata
    {
        [DisplayAttribute(Name = "WIN ID")]
        [Required(ErrorMessage = "Please enter WINID.")]
        public string ManagerWinId { get; set; }

        [Required(ErrorMessage = "Please enter First name.")]
        [StringLength(10, MinimumLength = 2)]
        [DisplayAttribute(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter Last name.")]
        [StringLength(10, MinimumLength = 1)]
        [DisplayAttribute(Name = "Last Name")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Please Enter Email Address")]
        [DisplayAttribute(Name = "Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        ErrorMessage = "Please Enter Correct Email Address")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Please enter Created Date Time.")]
        [DisplayAttribute(Name = "CDT")]
        public string CreatedDateTime { get; set; }

        [Required(ErrorMessage = "Please enter Modified Date Time.")]
        [DisplayAttribute(Name = "MDT")]
        public string ModifiedDateTime { get; set; }

        [Required(ErrorMessage = "Please enter Mobile Number.")]
        [DisplayAttribute(Name = "Mobile No")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Please enter Department")]
        [DisplayAttribute(Name = "Department")]
        public Int32 DepartmentId { get; set; }

        [Required(ErrorMessage = "Please enter City Name")]
        [DisplayAttribute(Name = "City Name")]
        public Int32 CityId { get; set; }

        [Required(ErrorMessage = "Please enter Country Name")]
        [DisplayAttribute(Name = "Country Name")]
        public Int32 CountryId { get; set; }

        [Required(ErrorMessage = "Please enter State Name")]
        [DisplayAttribute(Name = "State Name")]
        public Int32 StateId { get; set; }


        [Required(ErrorMessage = "Please Select Gender")]
        [DisplayAttribute(Name = "Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Please enter Address")]
        [DisplayAttribute(Name = "Address")]
        public string Address { get; set; }

    }
}