﻿using Microsoft.EntityFrameworkCore;
using ModsenLibDb.Enitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibDb
{
    public class ModsenLibAPIContext : DbContext
    {
        public DbSet<Book>? Books { get; set; }
        public DbSet<BookPassport>? BookPassports { get; set; }

        public ModsenLibAPIContext(DbContextOptions<ModsenLibAPIContext> options)
         : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookPassport>()
                .HasOne(bp => bp.Book)
                .WithOne(b => b.BookPassport)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
