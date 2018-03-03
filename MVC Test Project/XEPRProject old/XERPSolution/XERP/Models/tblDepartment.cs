using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace XEPR.Models
{
    [MetadataType(typeof(DepartmentMetadata))]
    public partial class tblDepartment
    {

    }
    public class DepartmentMetadata
    {
        public string DepartmentId;

        [DisplayAttribute(Name="Department")]
        [Required(ErrorMessage="Please enter Department name.")]
        public string DepartmentName { get; set; }

        [DisplayAttribute(Name = "Created By")]
        [Required(ErrorMessage = "Please enter Created By.")]
        public string CreatedBy { get; set; }
    }
}