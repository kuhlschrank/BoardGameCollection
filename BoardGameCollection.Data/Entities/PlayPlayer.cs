using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BoardGameCollection.Data.Entities
{
    [Table("PlayPlayer")]
    public class PlayPlayer
    {
        public long PlayId { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public int? UserId { get; set; }
        public decimal? Score { get; set; }
        public bool New { get; set; }
        public bool Win { get; set; }
        public string Color { get; set; }

        public Play Play { get; set; }
    }
}
