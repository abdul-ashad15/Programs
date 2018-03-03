using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EmployeeDataAccess;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Threading;

namespace WebAPIDemoProject.Controllers
{
    [RoutePrefix("api/Employees")]
    public class EmployeesController : ApiController
    {
        [Route("GetEmployees")]
        //[BasicAuthentication]
        public async Task<IHttpActionResult> Get()
        {
            IHttpActionResult result;
            TestEntities entities = new TestEntities();

            string userName = Thread.CurrentPrincipal.Identity.Name;
            var jsonString = JsonConvert.SerializeObject(entities.Employees.ToList());
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
            result = ResponseMessage(response);

            return result;
        }

        [Route("GetEmployee/{id}")]
        public async Task<IHttpActionResult> Get(int id)
        {
            IHttpActionResult result;
            using (TestEntities entities = new TestEntities())
            {
                var entity = entities.Employees.FirstOrDefault(e => e.ID == id);
                //if (entity != null)
                //{
                    var content = new StringContent(entity.ToString(), Encoding.UTF8, "application/json");
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
                    result = ResponseMessage(response);
                //}
                //else
                //{
                //    var response = new HttpRequestMessage(HttpStatusCode.NotFound, "Employee with Id" + " " + id.ToString() + " " + "not found");
                //}
                   
                return result;
            }
        }

        [Route("CreateEmployees")]
        public async Task<IHttpActionResult> Post([FromBody] Employee employee)
        {
            IHttpActionResult result;
            try
            {
                TestEntities entities = new TestEntities();

                entities.Employees.Add(employee);
                entities.SaveChanges();

                var content = new StringContent(entities.Employees.ToString(), Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = new HttpResponseMessage(HttpStatusCode.Created) { Content = content };
                result = ResponseMessage(response);
                return result;
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("DeleteEmployee/{id}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            //IHttpActionResult result;
            try
            {
                TestEntities entities = new TestEntities();
                var entity = entities.Employees.FirstOrDefault(em => em.ID == id);
                if(entity != null)
                {
                    entities.Employees.Remove(entity);
                    entities.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with Id = " + id.ToString() + " " + "not found to delete");
                }
                
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [Route("UpdateEmployee/{id}")]
        public async Task<HttpResponseMessage> Put(int id, [FromBody]Employee employee)
        {
            try
            {
                TestEntities entities = new TestEntities();
                var entity = entities.Employees.FirstOrDefault(em => em.ID == id);
                if( entity != null)
                {
                    entity.FirstName = employee.FirstName;
                    entity.LastName = employee.LastName;
                    entity.Gender = employee.Gender;
                    entity.Salary = employee.Salary;
                    entities.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with Id = " + id.ToString() + " " + "not found to update");
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
