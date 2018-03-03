using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDemo
{
    public abstract class AbstractDemo
    {
        int i = 10;

        
    }

    public class Demodispose : IDisposable
    {
        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // clean unmanged objects
                }
                // clean unmanaged objects).

                disposed = true;
            }
        }

        static void Main(string[] args)
        {
            Demodispose obj = new Demodispose();
            obj.Dispose();
        }
    }
}
