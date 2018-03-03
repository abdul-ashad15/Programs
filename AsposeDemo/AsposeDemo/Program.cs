using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.BarCode;

namespace AsposeDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Aspose.BarCode.License license = new Aspose.BarCode.License();
            license.SetLicense("Aspose.Total.lic");

            using(BarCodeReader bcr = new BarCodeReader(imageFile, BarCodeReadType.Interleaved2of5 | BarCodeReadType.Code128 |
                        BarCodeReadType.Code39Standard | BarCodeReadType.PatchCode |
                        BarCodeReadType.Pdf417 | BarCodeReadType.DataMatrix |
                        BarCodeReadType.SSCC18 | BarCodeReadType.Code93Extended | BarCodeReadType.Code39Extended))
        }
    }
}
