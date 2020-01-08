using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCollection.Web.Models
{
    public class GameTableGroupHeaderModel
    {
        public string Caption { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }

        public GameTableGroupHeaderModel(string caption, string left = "Titel", string right = "Spieler")
        {
            Caption = caption;
            Left = left;
            Right = right;
        }
    }
}
