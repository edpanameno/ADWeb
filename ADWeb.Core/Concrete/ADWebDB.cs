using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Core.Concrete
{
    using System.Data.Entity.ModelConfiguration.Conventions;
    using ADWeb.Core.Entities;

    public class ADWebDB : DbContext
    {
        public ADWebDB() : base("ADWebDB") { }

        public DbSet<DomainUser> DomainUsers { get; set; }
        public DbSet<UserUpdateHistory> UserUpdateHistory { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //modelBuilder.Entity<DomainUser>().HasKey(u => u.DomainUserID);
        }
    }
}
