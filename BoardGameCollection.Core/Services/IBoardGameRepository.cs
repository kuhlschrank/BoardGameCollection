using BoardGameCollection.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameCollection.Core.Services
{
    public interface IBoardGameRepository
    {
        void StoreBoardGames(IEnumerable<BoardGame> boardGames);
        void StoreUnknownIds(IEnumerable<int> unknownIds);
        IEnumerable<BoardGame> GetBoardGames(IEnumerable<int> ids);
        IEnumerable<BoardGame> GetNextBoardGamesToUpdate(int count, int minimumHoursPassed);
    }
}
