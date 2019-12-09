using BoardGameCollection.Core.GeekConnector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameCollection.Core.Services
{
    public interface IBoardGameCrawler
    {
        void AddUnknownBoardGameIds(IEnumerable<BoardGameId> ids);
    }
}
