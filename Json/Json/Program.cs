using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Json
{
    class Program
    {
        static void Main(string[] args)
        {
            //Serialize object to Json
            MyDetail name = new MyDetail();
            name.FirstName = "Abdul";
            name.LastName = "Ashad";
            var jsonstring = JsonConvert.SerializeObject(name);

            //DeSerialize Json string to particular object
            string jsonData = "{\"FirstName\":\"Abdul\",\"LastName\":\"Ashad\"}";
            var details = JsonConvert.DeserializeObject<MyDetail>(jsonData);


        }
    }

    public class MyDetail
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }  
}
