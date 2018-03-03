using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebAPIDemo.Controllers
{
    //[Authorize]
    [RoutePrefix("api/values")]
    public class ValuesController : ApiController
    {
        // GET api/values
        [Route("api/values")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        //[Route("api/values/id")]
        public string Get(int id)
        {
            if (id == 1)
                return "value1";
            else
                return "value2";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
