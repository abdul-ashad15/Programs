using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using WebAPIDemoProject.Models;


namespace WebAPIDemoProject.Controllers
{
    [RoutePrefix("api/students")]
    public class StudentsController : ApiController
    {
        [Route("GetStudents")]
        public IHttpActionResult Get()
        {
            IHttpActionResult _response;

            List<Students> students = new List<Students>();
            students = GetStudentsDetails();

            string jsonString = JsonConvert.SerializeObject(students);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
            return _response = ResponseMessage(response);
        }

        [Route("GetStudent/{id}")]
        public IHttpActionResult Get(int id)
        {
            List<Students> students = new List<Students>();
            IHttpActionResult _response;
           
            students = GetStudentsDetails();
            var student = students.FirstOrDefault(e => e.StudentId == id);

            if (student != null)
                return Ok<Students>(student);
            else
            {
                var content = new StringContent("Student with StudentId:" + " " + id + " " + "not found", Encoding.UTF8, "application/text");
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound) {Content = content };
                return _response = ResponseMessage(response);
            }
        }

        public List<Students> GetStudentsDetails()
        {
            List<Students> students = new List<Students>();
            students.Add(new Students { StudentId = 1, StudentName = "Ashad1" });
            students.Add(new Students { StudentId = 2, StudentName = "Ashad2" });
            students.Add(new Students { StudentId = 3, StudentName = "Ashad3" });
            students.Add(new Students { StudentId = 4, StudentName = "Ashad4" });

            return students;
        }
    }
}
