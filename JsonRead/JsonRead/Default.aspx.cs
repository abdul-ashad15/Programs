using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace JsonRead
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string file = "D:\\JsonDetails.json";
            string Json = System.IO.File.ReadAllText(file);
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            object serJsonDetails = javaScriptSerializer.DeserializeObject(Json);
            Dictionary<string, object> jsonDetails = serJsonDetails as Dictionary<string, object>;

        }
    }
}