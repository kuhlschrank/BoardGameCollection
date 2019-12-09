using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BoardGameCollection.Core.Models;

namespace BoardGameCollection.Core.Models
{
    public class OwnedEntry
    {
        public BoardGame BoardGame { get; set; }
        public string Owner { get; set; }
    }

    public class OwnedList
    {
        public List<OwnedEntry> OwnedEntries { get; set; }

        public OwnedList()
        {
            OwnedEntries = new List<OwnedEntry>();
        }

        public string Owners
        {
            get { return string.Join(" and ", OwnedEntries.Select(p => p.Owner).Distinct()); }
        }
    }
}