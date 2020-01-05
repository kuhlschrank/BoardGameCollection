using System;
using System.Collections.Generic;
using System.Text;

namespace BoardGameCollection.Core.Models
{
    public class Play
    {
        public long Id { get; set; }
        public int BoardGameId { get; set; }
        public string BoardGameName { get; set; }
        public string Comments { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
        public bool IgnoreForStatistics { get; set; }
        public string Location { get; set; }
        public List<PlayerPlayStats> Players { get; set; }
    }
}
