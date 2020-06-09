using ATEK.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.Data.Contexts
{
    public class AccessControlContext : DbContext
    {
        public AccessControlContext()
        {
        }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Gate> Gates { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Class> Classes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AccessControlAppData");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profile>().HasIndex(u => u.Pinno).IsUnique();
            modelBuilder.Entity<Gate>().HasIndex(u => u.FirebaseId).IsUnique();
            modelBuilder.Entity<ProfileGate>().HasKey(s => new { s.ProfileId, s.GateId });
            modelBuilder.Entity<ProfileGroup>().HasKey(s => new { s.ProfileId, s.GroupId });
        }
    }
}