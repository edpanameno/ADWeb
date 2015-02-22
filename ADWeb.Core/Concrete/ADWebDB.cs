using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Core.Concrete
{
    using ADWeb.Core.Entities;

    public class ADWebDB : DbContext
    {
        public ADWebDB() : base("ADWebDB") { }

        public DbSet<DomainUser> DomainUsers { get; set; }
    }
}
