using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace XEPR.Models
{
    public class CustomManagerView
    {

        public String ManagerWinId { get; set; }
        public int ManagerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public DateTime DOJ { get; set; }
        public string EmailAddress { get; set; }
        public string Department { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        [DisplayAttribute(Name = "Department Name")]
        public string DepartmentId { get; set; }
        public string CreatedBy { get; set; }
        public long MobileNo { get; set; }

        [DisplayAttribute(Name = "Country Name")]
        public string CountryId { get; set; }
        [DisplayAttribute(Name = "State Name")]
        public string StateId { get; set; }
        [DisplayAttribute(Name = "City Name")]
        public string CityId { get; set; }


    }
}