using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MVCDemoApplication.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [DisplayName("Name")]
        public string EmployeeName { get; set; }
    }
}