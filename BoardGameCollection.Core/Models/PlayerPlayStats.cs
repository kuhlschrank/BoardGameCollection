using System;
using System.Collections.Generic;
using System.Text;

namespace BoardGameCollection.Core.Models
{
    public class PlayerPlayStats
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public int? UserId { get; set; }
        public decimal? Score { get; set; }
        public bool New { get; set; }
        public bool Win { get; set; }
        public string Color { get; set; }
    }
}
