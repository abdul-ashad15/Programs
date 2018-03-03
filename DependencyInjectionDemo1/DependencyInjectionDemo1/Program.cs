using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionDemo1
{
    public class BusinessLogicImplementation
    {
        private IService client;
        public BusinessLogicImplementation(IService client)
        {
            this.client = client;
            Console.WriteLine("Constructor Injection Injection ==> Current Service: {0}",client.ServiceMethod());
            Console.ReadLine();

         }
        static void Main(string[] args)
        {
            ClaimService claimService = new ClaimService();
            AdjudicationService adjudicationService = new AdjudicationService();
            PaymentService paymentService = new PaymentService();
            BusinessLogicImplementation ConInjBusinessLogic = new BusinessLogicImplementation(claimService);
        }
    }

    public interface IService
    {
        string ServiceMethod();
    }

    public class ClaimService : IService
    {
        public string ServiceMethod()
        {
            return "ClaimService is running";
        }
    }
    public class AdjudicationService : IService
    {
        public string ServiceMethod()
        {
            return "AdjudicationService is running";
        }
    }

    public class PaymentService : IService
    {
        public string ServiceMethod()
        {
            return "PaymentService is running";
        }
    }
}
