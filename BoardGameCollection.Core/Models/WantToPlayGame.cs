using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BoardGameCollection.Core.Models;

namespace BoardGameCollection.Core.Models
{
    public class WantToPlayGame
    {
        public BoardGame BoardGame { get; set; }
        public string Owner { get; set; }
        public DateTime LastModified { get; set; }
    }
}