using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ADWeb.Core.DAL
{
    using ADWeb.Core.Entities;

    public class ADWebDB : DbContext
    {
        public ADWebDB() : base("ADWebDB") { }

        public DbSet<DomainUser> DomainUsers { get; set; }
        public DbSet<UserUpdateHistory> UserUpdateHistory { get; set; }
        public DbSet<DomainOU> DomainOU { get; set; }
        public DbSet<UserTemplate> UserTemplate { get; set; }
        public DbSet<UserTemplateGroup> UserTemplateGroup { get; set; }
        public DbSet<ADSetting> ADSetting { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
