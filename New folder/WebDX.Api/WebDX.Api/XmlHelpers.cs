using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace WebDX.Api
{
    public class XmlHelpers
    {
        public static WDECheckDigitMethods GetCheckDigitMethods(XmlTextReader reader, string attributeName)
        {
            return Utils.GetCheckDigitMethods(reader, attributeName);
        }
    }
}
