using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DemoAPI.Controllers
{
    [RoutePrefix("api/Students")]
    public class StudentsController : ApiController
    {
        List<string> students = new List<string>
        {
            "Student1","Student2","Student3","Student4"
        };
        

        [Route("GetStudents")]
        public IEnumerable<string> GetStudents()
        {
            return students;
        }

        [Route("GetStudent/id")]
        public string GetStudent(int id)
        {
            if (id == 1)
                return students[id - 1];
            else if (id == 2)
                return students[id - 1];
            else if (id == 3)
                return students[id - 1];
            else
                return students[id - 1];

        }
    }
}
