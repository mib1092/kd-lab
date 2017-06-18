using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using IMCMS.Models.DAL;
using IMCMS.Models.Entities;

namespace IMCMS.Models.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(DataContext context)
        {
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


            if (!context.Jobs.Any())
            {
                context.Jobs.Add(
                    new Entities.Job
                    {
                        BaseID = 1,
                        Created = DateTime.Now,
                        Who = "SYSTEM",
                        Status = VersionableItemStatus.Live,
                        Visbility = VersionableVisbility.Public,
                        Title = "Other - Driving",
                        City = "N/A",
                        State = "N/A",
                        JobType = JobType.Driving,
                        ListOnWebsite = false,
                        ListOnIndeed = false,
                        IsDefault = true,
                        Order = 1
                    });
                context.Jobs.Add(
                    new Entities.Job
                    {
                        BaseID = 2,
                        Created = DateTime.Now,
                        Who = "SYSTEM",
                        Status = VersionableItemStatus.Live,
                        Visbility = VersionableVisbility.Public,
                        Title = "Other - Non-Driving",
                        City = "N/A",
                        State = "N/A",
                        JobType = JobType.Non_Driving,
                        ListOnWebsite = false,
                        ListOnIndeed = false,
                        IsDefault = true,
                        Order = 1
                    });
            }

            if (!context.EmployeePortalSettings.Any())
            {
                context.EmployeePortalSettings.Add(
                    new EmployeePortalSettings
                    {
                        BaseID = 1,
                        Status = VersionableItemStatus.Live,
                        Visbility = VersionableVisbility.Public,
                        Who = "System",
                        Created = DateTime.Now,
                        Password = IMCMS.Common.Hashing.AESEncrypt.Encrypt("W0ZhGbuv")
                    });
            }

            if (!context.EmployeePortalPages.Any())
            {
                context.EmployeePortalPages.AddOrUpdate(
                    p => p.ID,
                    new EmployeePortalPage
                    {
                        Title = "Helpful Links",
                        ParentId = null,
                        Order = 1,
                        BaseID = 1,
                        Status = VersionableItemStatus.Live,
                        Visbility = VersionableVisbility.Public,
                        Created = DateTime.Now,
                        Slug = "helpful-links",
                        PageType = PortalType.Standard,
                        Who = "System"
                    },
                    new EmployeePortalPage
                    {
                        Title = "Manage Benefits",
                        ParentId = null,
                        Order = 2,
                        BaseID = 2,
                        Status = VersionableItemStatus.Live,
                        Visbility = VersionableVisbility.Public,
                        Created = DateTime.Now,
                        Slug = "manage-benefits",
                        PageType = PortalType.Standard,
                        Who = "System"
                    }
                );
            }
        }
    }
}
