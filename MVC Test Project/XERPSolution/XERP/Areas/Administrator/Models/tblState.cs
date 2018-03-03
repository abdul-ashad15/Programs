using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XEPR.Models
{

    [MetadataType(typeof(StateMetadata))]
    public partial class tblState
    {

    }
    public partial class StateMetadata
    {
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string StateName { get; set; }
    }

}