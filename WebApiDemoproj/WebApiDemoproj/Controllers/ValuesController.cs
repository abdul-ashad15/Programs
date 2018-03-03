using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiDemoproj.Controllers
{
    //[Authorize]
    public class ValuesController : ApiController
    {

        public List<Employee> GetEmployees()
        {
           
            List<Employee> employees = new List<Employee>();


            return employees;
        }
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            if (id == 1)
                return "value1";
            else
                return "value";
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

    public class Employee
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
    }
}
