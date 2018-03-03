using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using System.Web.Mvc.Async;
using XEPR.API.Models;



//using System.Web.Mvc;

namespace XEPR.API.Controllers
{
    public class ValuesController : ApiController
    {
        [HttpGet]
        public IEnumerable<clsCountry> GetAllCountry()
        {
            clsCountry C1 = new clsCountry();
            clsCountry C2 = new clsCountry();
            List<clsCountry> lstCountry = new List<clsCountry>();

            C1.CountryName = "India";
            lstCountry.Add(C1);
            C2.CountryName = "Pakistan";
            lstCountry.Add(C2);

            return lstCountry;
        }

        [HttpPost]
        public void PostCountry([FromBody]clsCountry c)
        {

        }

        [HttpPut]
        public void PutCountry(int id, [FromBody]clsCountry c)
        {
        }

        [HttpDelete]
        public void DeleteCountry(int id)
        {

        }
    }
}