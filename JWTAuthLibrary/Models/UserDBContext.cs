using JWTAuthLibrary;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace JWTLibrary
{
    public class UserDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=UserDB.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>(u => {
                u.HasIndex(item => item.Name).IsUnique();
                u.Property(u => u.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<Roles>(r => {
                r.HasIndex(r => r.Name).IsUnique();
                r.Property(r => r.IsActive).HasDefaultValue(true);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}