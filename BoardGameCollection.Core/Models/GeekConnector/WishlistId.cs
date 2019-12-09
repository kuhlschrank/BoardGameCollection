using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameCollection.Core.GeekConnector.Models
{
    public class WishlistId : BoardGameId
    {
        public byte Priority { get; set; }
        public string Comment { get; set; }
    }
}
