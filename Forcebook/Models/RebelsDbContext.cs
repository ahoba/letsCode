using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forcebook.Models
{
    public class RebelsDbContext : DbContext
    {
        public DbSet<Rebel> Rebels { get; set; }

        public DbSet<ItemTemplate> ItemTemplates { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<TreasonReport> TreasonReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>(item =>
            {
                item.HasOne(i => i.Owner)
                    .WithMany(r => r.Inventory)
                    .HasForeignKey(i => i.OwnerId);

                item.HasOne(i => i.Template)
                    .WithMany()
                    .HasForeignKey(i => i.TemplateId);
            });

            modelBuilder.Entity<TreasonReport>(report =>
            {
                report.HasOne(r => r.Accused).WithMany(r => r.TreasonReports).HasForeignKey(r => r.AccusedId);

                report.HasOne(r => r.Accuser).WithMany().HasForeignKey(r => r.AccuserId);
            });

            SeedRebels(modelBuilder);

            SeedItemTemplates(modelBuilder);

            SeedItems(modelBuilder);

            SeedTreasonReports(modelBuilder);
        }

        private void SeedTreasonReports(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TreasonReport>().HasData(
                new TreasonReport()
                {
                    Id = 1,
                    AccuserId = "Luke Skywalker",
                    AccusedId = "Boba Fett"
                });
            modelBuilder.Entity<TreasonReport>().HasData(
                new TreasonReport()
                {
                    Id = 2,
                    AccuserId = "Leia Organa",
                    AccusedId = "Boba Fett"
                });
            modelBuilder.Entity<TreasonReport>().HasData(
                new TreasonReport()
                {
                    Id = 3,
                    AccuserId = "Han Solo",
                    AccusedId = "Boba Fett"
                });
        }

        private void SeedItems(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>().HasData(
                new Item()
                {
                    Id = 1,
                    OwnerId = "Luke Skywalker",
                    TemplateId = "Weapon",
                    Quantity = 10
                });

            modelBuilder.Entity<Item>().HasData(
                new Item()
                {
                    Id = 2,
                    OwnerId = "Luke Skywalker",
                    TemplateId = "Ammo",
                    Quantity = 10
                });

            modelBuilder.Entity<Item>().HasData(
                new Item()
                {
                    Id = 3,
                    OwnerId = "Luke Skywalker",
                    TemplateId = "Water",
                    Quantity = 10
                });

            modelBuilder.Entity<Item>().HasData(
                new Item()
                {
                    Id = 4,
                    OwnerId = "Luke Skywalker",
                    TemplateId = "Food",
                    Quantity = 10
                });

            modelBuilder.Entity<Item>().HasData(
                new Item()
                {
                    Id = 5,
                    OwnerId = "Leia Organa",
                    TemplateId = "Weapon",
                    Quantity = 10
                });

            modelBuilder.Entity<Item>().HasData(
                new Item()
                {
                    Id = 6,
                    OwnerId = "Leia Organa",
                    TemplateId = "Ammo",
                    Quantity = 10
                });

            modelBuilder.Entity<Item>().HasData(
                new Item()
                {
                    Id = 7,
                    OwnerId = "Leia Organa",
                    TemplateId = "Water",
                    Quantity = 10
                });

            modelBuilder.Entity<Item>().HasData(
                new Item()
                {
                    Id = 8,
                    OwnerId = "Leia Organa",
                    TemplateId = "Food",
                    Quantity = 10
                });

            modelBuilder.Entity<Item>().HasData(
                new Item()
                {
                    Id = 9,
                    OwnerId = "Boba Fett",
                    TemplateId = "Weapon",
                    Quantity = 5
                });
        }

        private void SeedItemTemplates(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemTemplate>().HasData(
                new ItemTemplate()
                {
                    Name = "Weapon",
                    Points = 4
                });
            modelBuilder.Entity<ItemTemplate>().HasData(
                new ItemTemplate()
                {
                    Name = "Ammo",
                    Points = 3
                });
            modelBuilder.Entity<ItemTemplate>().HasData(
                new ItemTemplate()
                {
                    Name = "Water",
                    Points = 2
                });
            modelBuilder.Entity<ItemTemplate>().HasData(
                new ItemTemplate()
                {
                    Name = "Food",
                    Points = 1
                });
        }

        private void SeedRebels(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rebel>().HasData(
                new Rebel()
                {
                    Name = "Luke Skywalker",
                    Age = 22,
                    Gender = Gender.Male,
                    Location = new Location()
                    {
                        Name = "Hoth",
                        Latitude = 123,
                        Longitude = 234
                    },
                    TreasonReports = new List<TreasonReport>()
                    {

                    }
                });

            modelBuilder.Entity<Rebel>().HasData(
                new Rebel()
                {
                    Name = "Leia Organa",
                    Age = 22,
                    Gender = Gender.Female,
                    Location = new Location()
                    {
                        Name = "Alderaan",
                        Latitude = 234,
                        Longitude = 345
                    }
                });

            modelBuilder.Entity<Rebel>().HasData(
                 new Rebel()
                 {
                     Name = "Han Solo",
                     Age = 35,
                     Gender = Gender.Male,
                     Location = new Location()
                     {
                         Name = "Tatooine",
                         Latitude = 345,
                         Longitude = 456
                     }
                 });

            modelBuilder.Entity<Rebel>().HasData(
                new Rebel()
                {
                    Name = "Chewbacca",
                    Age = 203,
                    Gender = Gender.Male,
                    Location = new Location()
                    {
                        Name = "Tatooine",
                        Latitude = 345,
                        Longitude = 456
                    }
                });

            modelBuilder.Entity<Rebel>().HasData(
                 new Rebel()
                 {
                     Name = "Boba Fett",
                     Age = 35,
                     Gender = Gender.Male,
                     Location = new Location()
                     {
                         Name = "Kamino",
                         Latitude = 456,
                         Longitude = 567
                     }
                 });
        }

        public RebelsDbContext(DbContextOptions<RebelsDbContext> options) : base(options)
        {

        }
    }
}
