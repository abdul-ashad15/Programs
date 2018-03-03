using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace XEPR.Models
{
    [MetadataType(typeof(CountryMetadata))]
    public partial class tblCountry
    {
       
    }
    public class CountryMetadata
    {

        public int CountryId { get; set; }
        public string CountryName { get; set; }
          
    }
}