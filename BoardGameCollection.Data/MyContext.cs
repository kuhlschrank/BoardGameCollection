using BoardGameCollection.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoardGameCollection.Data
{
    public class MyContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "");
        }

        public DbSet<BoardGame> BoardGames { get; set; }
        public DbSet<Unknown> Unknowns { get; set; }
    }
}
