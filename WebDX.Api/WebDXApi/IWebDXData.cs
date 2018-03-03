using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDXApi
{
    public interface IWebDXData
    { 
        //void SaveFile(string filePath);

        int CurrentSWVersion(int x);
    }
}
