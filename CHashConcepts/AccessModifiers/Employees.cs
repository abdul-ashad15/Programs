using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessModifiers
{
    public class Employees
    {
        public int EmplyoyeeId { get; set; }
        private string EmplyoeeName { get; set; }
        internal int RollNumber { get; set; }
        public int Salary { get; }

    }
}
