using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client(new  Service());
            client
        }
    }

    public interface IService
    {
        void Serve();
    }

    public class Service : IService
    {
        public void Serve()
        {
            Console.WriteLine("Serve Method Called");
        }
    }

    public class Client
    {
        private IService _Service;

        public Client(IService service)
        {
            _Service = service;
        }
    }
}
