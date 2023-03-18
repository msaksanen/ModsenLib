using Microsoft.EntityFrameworkCore;
using ModsenLibGateWayDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibGateWayDb
{
    public class ModsenLibGateWayContext : DbContext
    {
        public DbSet<User>? Users { get; set; }
        public DbSet<Role>? Roles { get; set; }

        public DbSet<RefreshToken>? RefreshTokens { get; set; }

        public ModsenLibGateWayContext(DbContextOptions<ModsenLibGateWayContext> options)
       : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>()
                .HasOne(u => u.User)
                .WithMany(c => c.RefreshTokens)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
