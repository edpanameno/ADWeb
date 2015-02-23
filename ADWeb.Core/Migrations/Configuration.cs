namespace ADWeb.Core.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using ADWeb.Core.Entities;

    internal sealed class Configuration : DbMigrationsConfiguration<ADWeb.Core.Concrete.ADWebDB>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ADWeb.Core.Concrete.ADWebDB context)
        {
            context.DomainUsers.AddOrUpdate(
                u => u.UserName,
                new DomainUser { CreatedBy = "administrator", DateCreated = DateTime.Now, UserName = "pebble12", Enabled = true, FirstName = "Pebble", LastName = "Stone" },
                new DomainUser { CreatedBy = "administrator", DateCreated = DateTime.Now, UserName = "rick.grimes", Enabled = true, FirstName = "Rick", LastName = "Grimes" },
                new DomainUser { CreatedBy = "administrator", DateCreated = DateTime.Now, UserName = "data.android", Enabled = true, FirstName = "Data", LastName = "Android" },
                new DomainUser { CreatedBy = "administrator", DateCreated = DateTime.Now, UserName = "wesly.crusher", Enabled = true, FirstName = "Wesley", LastName = "Crusher", MiddleName = "Felix" }
            );
            
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
