using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BoardGameCollection.Data.Entities
{
    [Table("Play")]
    public class Play
    {
        public Play()
        {
            Players = new List<PlayPlayer>();
        }

        [Key]
        public long Id { get; set; }
        public int BoardGameId { get; set; }
        public string Comments { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
        public bool IgnoreForStatistics { get; set; }
        public string Location { get; set; }
        public ICollection<PlayPlayer> Players { get; set; }
    }
}
