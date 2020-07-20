using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronicle.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Chronicle.Web.Data
{
    public class ChronicleContext : DbContext
    {

        public ChronicleContext(DbContextOptions<ChronicleContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Chronicle>()
                .HasOne(c => c.User)
                .WithMany(u => u.OwnedChronicles);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Chronicle)
                .WithMany(c => c.Posts);


            modelBuilder.Entity<Photo>()
                .HasOne(ph => ph.Post)
                .WithOne(p => p.Photo);

        }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Models.Chronicle> Chronicles { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<Family> Families { get; set; }
        public virtual DbSet<FamilyMember> FamilyMembers { get; set; }
    }
}
