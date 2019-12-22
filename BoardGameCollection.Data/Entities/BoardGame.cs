using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameCollection.Data.Entities
{
    [Table("BoardGame")]
    public class BoardGame
    {
        public BoardGame()
        {
            Expansions = new List<Expansion>();
        }

        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string ThumbnailUri { get; set; }
        public string ImageUri { get; set; }
        public int? MinPlayers { get; set; }
        public int? MaxPlayers { get; set; }
        public int? YearPublished { get; set; }
        public int? BestPlayerNumber { get; set; }
        public double? AverageRating { get; set; }
        public DateTimeOffset LastUpdate { get; set; }

        public List<Expansion> Expansions { get; set; }
    }
}
