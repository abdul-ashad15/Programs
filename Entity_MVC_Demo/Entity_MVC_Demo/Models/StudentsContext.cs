using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Entity_MVC_Demo.Models
{
    public class StudentsContext : DbContext
    {
        public DbSet<Students> students { get; set; }
    }
}