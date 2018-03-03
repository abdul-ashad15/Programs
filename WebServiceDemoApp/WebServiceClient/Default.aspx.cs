using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebServiceClient
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CalculatorService.CalculatorServiceSoapClient service = new CalculatorService.CalculatorServiceSoapClient();
            int result = service.Addition(10, 20);
        }
    }
}