using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Entity_MVC_Demo.Models
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(): base("name=SchoolDBConnectionString")
        {
            Database.SetInitializer<SchoolContext>(new DropCreateDatabaseIfModelChanges<SchoolContext>());
            
            //Database.SetInitializer<SchoolContext>(new CreateDatabaseIfNotExists<SchoolContext>());
            //Database.SetInitializer<SchoolContext>(new DropCreateDatabaseIfModelChanges<SchoolContext>());
            //Database.SetInitializer<SchoolContext>(new DropCreateDatabaseAlways<SchoolContext>());
            //Database.SetInitializer<SchoolContext>(new SchoolDBInitializer());
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Standard> Standards { get; set; }
    }
}