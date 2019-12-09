using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BoardGameCollection.Core.Models;

namespace BoardGameCollection.Core.Models
{
    public class GameWish
    {
        public BoardGame BoardGame { get; set; }
        public int Priority { get; set; }
        public string Owner { get; set; }
        public string Comment { get; set; }
    }
}