using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionhandlingDemo
{
    //class ExceptionsEx //Raise Exception 
    //{
    //    //001: Start the Program Execution.
    //    public void StartProgram()
    //    {
    //        Console.WriteLine("Making Call to F1() ");
    //        try
    //        {
    //            F1();
    //        }
    //        catch
    //        {
    //            Console.WriteLine("Some Exception Occurred. I don't know what Exception it is and where it Occurred. Sorry!");
    //        }
    //        Console.WriteLine("Successfully returned from F1() ");
    //    }

    //    //002: Set of Function that calls each other
    //    public void F1()
    //    {
    //        Console.WriteLine("Making Call to F2() ");
    //        throw new System.Exception();
    //        F2();
    //        Console.WriteLine("Successfully returned from F2() ");

    //    }
    //    public void F2()
    //    {
    //        Console.WriteLine("Inside F2 ");
    //    }

    //    //Client 001: Program Entry
    //    [STAThread]
    //    static void Main(string[] args)
    //    {
    //        //Create the Object and start the Execution
    //        ExceptionsEx app = new ExceptionsEx();
    //        app.StartProgram();
    //        Console.ReadLine();
    //    }
    //}

    //class ExceptionsEx //Exception Bubbling
    //{
    //    //001: Start the Program Execution.
    //    public void StartProgram()
    //    {
    //        Console.WriteLine("Making Call to F1() ");
    //        try
    //        {
    //            F1();
    //        }
    //        catch
    //        {
    //            Console.WriteLine("Some Exception Occurred. I don't know what Exception it is and where it Occurred. Sorry!");
    //        }
    //        Console.WriteLine("Successfully returned from F1() ");
    //    }

    //    //002: Set of Function that calls each other
    //    public void F1()
    //    {
    //        Console.WriteLine("Making Call to F2() ");
    //        F2();
    //        Console.WriteLine("Successfully returned from F2() ");
    //    }
    //    public void F2()
    //    {
    //        try
    //        {
    //            Console.WriteLine("Inside F2");
    //            F3();
    //            Console.WriteLine("Successfully returned from F2() ");
    //        }
    //        finally
    //        {
    //            Console.WriteLine("In Finally ");
    //        }

    //    }
    //    public void F3()
    //    {
    //        Console.WriteLine("Inside F3 ");
    //        throw new System.Exception();
    //    }

    //    //Client 001: Program Entry
    //    [STAThread]
    //    static void Main(string[] args)
    //    {
    //        //Create the Object and start the Execution
    //        ExceptionsEx app = new ExceptionsEx();
    //        app.StartProgram();
    //        Console.ReadLine();
    //    }
    //}

    //001: Function that Handles the Exception


   public class ExceptionsP2
    {
        public void StartProgram()
        {
            Console.WriteLine("Calling the Function Calculate");
            try
            {
                Calculate();
            }
            catch (DivideByZeroException Ex)
            {
                Console.WriteLine("Divide By Zero.Look at the Call stack for More information");

                Console.WriteLine("Packed Message: " + Ex.Message);
                Console.WriteLine("Thrown By: " + Ex.TargetSite);
                Console.Write("Call Stack: " + Ex.StackTrace);
            }
            catch (Exception Ex)
            {
                Console.WriteLine("General Exception Occurred");
                Console.WriteLine("Packed Message: " + Ex.Message);
                Console.Write("Call Stack: " + Ex.StackTrace);
            }
        }
        public void Calculate()
        {
            Console.WriteLine("Calling Divide Function ");
            Divide();
        }

        //003: The Divide function which actually raises an Exception
        public void Divide()
        {
            int x = 10;
            int y = 0;
            y = x / y;
        }
        [STAThread]
        public static void Main(string[] args)
        {
            ExceptionsP2 start = new ExceptionsP2();
            start.StartProgram();
        }
    }
}
