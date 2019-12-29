using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGameCollection.Core.Models
{
    public class BoardGame
    {
        public int Id { get; set; }
        public bool IsExpansion { get; set; }
        public string Title { get; set; }
        public string ThumbnailUri { get; set; }
        public string ImageUri { get; set; }
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public int YearPublished { get; set; }
        public List<int> ExpansionIds { get; set; } = new List<int>();
        public string[] SuggestedPlayerNumbers { get; set; }
        public IEnumerable<string> BestWithPlayerNumbers => SuggestedPlayerNumbers.Where(n => !n.StartsWith('-'));
        public double AverageRating { get; set; }

        public DateTime LastUpdated { get; set; }

        public bool IsBestWith(int playerNumber)
        {
            if (playerNumber > MaxPlayers || playerNumber < MinPlayers)
                return false;

            var bestWithNumbers = BestWithPlayerNumbers.ToList();
            var playerNumberString = playerNumber.ToString();
            if (bestWithNumbers.Any(n => n == playerNumberString))
                return true;
            
            return bestWithNumbers
                .Where(n => n.EndsWith('+'))
                .Select(n => Int32.Parse(n.TrimEnd('+')))
                .Any(n => n <= playerNumber);
        }

        public override string ToString()
        {
            return $"{Id} - {Title}";
        }
    }
}