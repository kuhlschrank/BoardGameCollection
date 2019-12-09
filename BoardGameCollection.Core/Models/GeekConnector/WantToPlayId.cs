using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameCollection.Core.GeekConnector.Models
{
    public class WantToPlayId : BoardGameId
    {
        public DateTime LastModified { get; set; }
    }
}
