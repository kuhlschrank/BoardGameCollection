using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGameCollection.Core.Models
{
    public class BoardGame
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ThumbnailUri { get; set; }
        public string ImageUri { get; set; }
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public int YearPublished { get; set; }
        public List<int> ExpansionIds { get; set; } = new List<int>();
        public int BestPlayerNumber { get; set; }
        public string[] BestPlayerNumbers { get; set; }
        public double AverageRating { get; set; }

        public DateTime LastUpdated { get; set; }

        public override string ToString()
        {
            return $"{Id} - {Title}";
        }
    }
}