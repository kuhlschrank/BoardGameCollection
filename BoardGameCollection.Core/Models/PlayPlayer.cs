using System;
using System.Collections.Generic;
using System.Text;

namespace BoardGameCollection.Core.Models
{
    public class PlayPlayer
    {
        /// <summary>
        /// Fake Primary Key
        /// </summary>
        public int Position { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public int? UserId { get; set; }
        public decimal? Score { get; set; }
        public bool New { get; set; }
        public bool Win { get; set; }
        public string Color { get; set; }
    }
}
