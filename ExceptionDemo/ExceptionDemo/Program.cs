using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionDemo
{
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        try
    //        {
    //            //int x = 10, y = 0, result;
    //            //result = x / y;
    //            FileStream fs = new FileStream(@"D:\Projects\ExceptionDemo\ExceptionDemo\Demo.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
    //            fs.Seek(0, SeekOrigin.Begin);

    //            //Console.WriteLine(result);
    //            //Console.ReadLine();
    //        }
    //        catch(Exception ex)
    //        {
    //            Console.WriteLine(ex.Message);
    //            Console.ReadLine();
    //        }

    //    }
    //}

    class ExceptionsEx
    {
        //001: Start the Program Execution.
        public void StartProgram()
        {
            Console.WriteLine("Making Call to F1() ");
            try
            {
                F1();
            }
            catch
            {
                Console.WriteLine("Some Exception Occurred. I don't know what Exception it is and where it Occurred. Sorry!");
            }
            Console.WriteLine("Successfully returned from F1() ");
        }

        //002: Set of Function that calls each other
        public void F1()
        {
            Console.WriteLine("Making Call to F2() ");
            F2();
            Console.WriteLine("Successfully returned from F2() ");
        }
        public void F2()
        {
            Console.WriteLine("Making Call to F2() ");
            F3();
            Console.WriteLine("Successfully returned from F2() ");
        }
        public void F3()
        {
            Console.WriteLine("Inside F3 ");
            throw new System.Exception();
        }

        //Client 001: Program Entry
        [STAThread]
        static void Main(string[] args)
        {
            //Create the Object and start the Execution
            ExceptionsEx app = new ExceptionsEx();
            app.StartProgram();
        }
    }
}
