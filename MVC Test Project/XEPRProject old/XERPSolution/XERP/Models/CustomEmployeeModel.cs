using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XEPR.Models
{
    public class CustomEmployeeModel
    {
        public int EmployeeId { get; set; }

        [DisplayAttribute(Name = "WIN ID")]
        public string EmpWinId { get; set; }

        [DisplayAttribute(Name = "First Name")]
        public string FirstName { get; set; }

        [DisplayAttribute(Name = "Last Name")]
        public string LastName { get; set; }

        [DisplayAttribute(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [DisplayAttribute(Name = "Gender")]
        [Required(ErrorMessage = "Please enter Gender.")]
        public string Gender { get; set; }

        [DisplayAttribute(Name = "Date of Joining")]
        public DateTime DOJ { get; set; }

        [DisplayAttribute(Name = "Address")]
        public string Address { get; set; }

        [DisplayAttribute(Name = "EmailAddress")]
        public string EmailAddress { get; set; }

        [DisplayAttribute(Name = "MobileNo")]
        public long MobileNo { get; set; }

        [DisplayAttribute(Name = "Department")]
        public string Department { get; set; }

        [DisplayAttribute(Name = "City Name")]
        public string CityName { get; set; }

        [DisplayAttribute(Name = "Country Name")]
        public string CountryName { get; set; }

        [DisplayAttribute(Name = "State Name")]
        public string StateName { get; set; }

            
    }
}