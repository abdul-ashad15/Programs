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
        [DisplayAttribute(Name="Department")]
        [Required(ErrorMessage="Please enter Department name.")]
        public string DepartmentName { get; set; }
    }
}