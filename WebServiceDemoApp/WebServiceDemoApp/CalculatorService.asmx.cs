using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WebServiceDemoApp
{
   [WebService]
    public class CalculatorService : System.Web.Services.WebService
    {

        [WebMethod]
        public int  Addition(int x,int y)
        {
            return x + y;
        }

        [WebMethod]
        public int Multiplication(int x, int y)
        {
            return x * y;
        }

        public int Division(int x, int y)
        {
            return x / y;
        }

        public int Substraction(int x, int y)
        {
            return x - y;
        }
    }
}
