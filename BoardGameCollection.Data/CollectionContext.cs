using BoardGameCollection.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace BoardGameCollection.Data
{
    class CollectionContext : DbContext
    {
        private readonly string _connectionString;

        public CollectionContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Expansion>()
                .HasKey(e => new { e.BoardGameId, e.ExpansionId });
            modelBuilder.Entity<PlayPlayer>()
                .HasKey(e => new { e.PlayId, e.Position });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        public DbSet<BoardGame> BoardGames { get; set; }
        public DbSet<Expansion> Expansions { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Unknown> Unknowns { get; set; }
        public DbSet<Play> Plays { get; set; }
        public DbSet<PlayPlayer> PlayPlayers { get; set; }
    }
}
