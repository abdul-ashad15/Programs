using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace XEPR.Models
{
    [MetadataType(typeof(AdminMetadata))]
    public partial class tblAdmin
    {
        public class AdminMetadata
        {
            [DisplayAttribute(Name = "User Name")]
            [Required(ErrorMessage = "Please enter User Name.")]
            public string UserName { get; set; }

            [DisplayAttribute(Name = "Password")]
            [Required(ErrorMessage = "Please enter Password.")]
            public string Password { get; set; }
        }
    }
}