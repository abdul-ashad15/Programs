using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XEPR.Common
{
    public class HomeViewModel : ViewModelBase
    {

    }
    public abstract class ViewModelBase
    {
        public string Name { get; set; }
    }
   
}