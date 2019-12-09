using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BoardGameCollection.Core.Models;

namespace BoardGameCollection.Core.Models
{
    public class GamePossession
    {
        public BoardGame BoardGame { get; set; }
        public string Owner { get; set; }
    }
}