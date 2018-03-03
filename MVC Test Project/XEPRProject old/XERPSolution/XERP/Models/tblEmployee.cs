using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace XEPR.Models
{
    [MetadataType(typeof(EmployeeMetadata))]
    public partial class tblEmployee
    {
        public class EmployeeMetadata
        {
            [DisplayAttribute(Name = "WIN ID")]
            [Required(ErrorMessage = "Please enter WINID.")]
            public string EmpWinId { get; set; }

            [DisplayAttribute(Name = "First Name")]
            [Required(ErrorMessage = "Please enter FisrtName.")]
            [StringLength(20, MinimumLength = 2)]
            public string FirstName { get; set; }

            [DisplayAttribute(Name = "Last Name")]
            [Required(ErrorMessage = "Please enter LastName.")]
            [StringLength(20, MinimumLength = 1)]
            public string LastName { get; set; }

            [DisplayAttribute(Name = "Date of Birth")]
            [Required(ErrorMessage = "Please enter Date of Birth.")]
            public DateTime DOB { get; set; }

            [DisplayAttribute(Name = "Gender")]
            [Required(ErrorMessage = "Please enter Gender.")]
            public string Gender { get; set; }

            [DisplayAttribute(Name = "Date of Joining")]
            [Required(ErrorMessage = "Please enter Date of Joining.")]
            public DateTime DOJ { get; set; }

            [DisplayAttribute(Name = "Address")]
            [Required(ErrorMessage = "Please enter Address.")]
            public string Address { get; set; }

            [DisplayAttribute(Name = "EmailAddress")]
            [Required(ErrorMessage = "Please enter EmailAddress.")]
            [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
          ErrorMessage = "Please Enter Correct Email Address")]
            public string EmailAddress { get; set; }

            [DisplayAttribute(Name = "MobileNo")]
            [Required(ErrorMessage = "Please enter MobileNo.")]
           
            public long MobileNo { get; set; }

            [DisplayAttribute(Name = "Department")]
            [Required(ErrorMessage = "Please Select Department.")]
            public string DepartmentId { get; set; }

            [DisplayAttribute(Name = "City Name")]
            [Required(ErrorMessage = "Please Select City Name.")]
            public int CityId { get; set; }

            [DisplayAttribute(Name = "Country Name")]
            [Required(ErrorMessage = "Please Select Country Name.")]
            public int CountryId { get; set; }

            [DisplayAttribute(Name = "State Name")]
            [Required(ErrorMessage = "Please Select State Name.")]
            public int StateId { get; set; }
            

        }
    }
}