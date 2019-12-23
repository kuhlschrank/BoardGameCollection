using BoardGameCollection.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGameCollection.Web.Models
{
    public class GamePlayWantage
    {
        public BoardGame BoardGame { get; set; }
        public List<string> Owners { get; set; }
        public DateTime LastModified { get; set; }
    }
}