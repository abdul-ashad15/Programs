﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
This project uses Automatic Package Restore feature of NuGet to resolve Aspose.Email for .NET API reference 
when the project is build. Please check https://Docs.nuget.org/consume/nuget-faq for more information. 
If you do not wish to use NuGet, you can manually download Aspose.Email for .NET API from http://www.aspose.com/downloads, 
install it and then add its reference to this project. For any issues, questions or suggestions 
please feel free to contact us using http://www.aspose.com/community/forums/default.aspx
*/

namespace Aspose.Email.Examples.CSharp.Email.Knowledge.Base
{
    class PrintMessageBody
    {
        public static void Run()
        {
            string dataDir = RunExamples.GetDataDir_KnowledgeBase();

            // ExStart:PrintMessageBody

            // Declare message as an MailMessage instance
            MailMessage message = new MailMessage();
            message.From = "user1@domain.com";
            message.To = "user2@domain.com";
            message.Subject = "My First Mail";
            message.Date = DateTime.Now;
            message.Body = "Text is the Mail Message";

            // Instantiate an instance of MailPrinter and Set the MessageFormattingFlags to none to display only the message body
            var printer = new Aspose.Email.Printing.MailPrinter();
            printer.FormattingFlags = Aspose.Email.Printing.MessageFormattingFlags.None;

            // Print the email to an XPS and TIFF file
            printer.Print(message, dataDir + "PrintXPS_out.xps", Aspose.Email.Printing.PrintFormat.XPS);
            printer.Print(message, dataDir + "PrintTIFF_out.tiff", Aspose.Email.Printing.PrintFormat.Tiff);
            // ExEnd:PrintMessageBody
        }
    }
}
