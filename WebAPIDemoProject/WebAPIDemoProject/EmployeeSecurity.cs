using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmployeeDataAccess;

namespace WebAPIDemoProject
{
    public class EmployeeSecurity
    {
        public static bool Login(string userName, string password)
        {
            TestEntities entities = new TestEntities();
            return entities.Users.Any(usr => usr.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) &&
            usr.Password.Equals(password));
        }
    }
}