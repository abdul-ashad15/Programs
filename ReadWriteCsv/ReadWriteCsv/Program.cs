using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadWriteCsv
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = @"D:\\videoCampaign.json";
            //deserialize JSON from file  
            string Json = System.IO.File.ReadAllText(file);
            JavaScriptSerializer ser = new JavaScriptSerializer();
            var personlist = ser.Deserialize<List<Person>>(Json);
        }
    }
}
