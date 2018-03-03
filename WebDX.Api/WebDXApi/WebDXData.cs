using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDXApi
{
    public class WebDXData : IWebDXData
    {
        string filePath = @"D:\Projects\WebDX.Api\WebDXApi\DataFile.xml";

        public int CurrentSWVersion(int x)
        {
            return 5;
        }

        //public void SaveFile(string filePath)
        //{

        //}
    }
}
