using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XEPR.Models
{
    [MetadataType(typeof(CityMetadata))]
    public partial class tblCity
    {

    }
    public partial class CityMetadata
    {
        public int CityId { get; set; }
        public int StateId { get; set; }
        public string CityName { get; set; }
    }
}