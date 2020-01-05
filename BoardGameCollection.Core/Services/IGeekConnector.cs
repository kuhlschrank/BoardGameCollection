using BoardGameCollection.Core.GeekConnector.Models;
using BoardGameCollection.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BoardGameCollection.Core.Services
{
    public interface IGeekConnector
    {
        IEnumerable<WishlistId> GetWishlistGameIds(string username);
        IEnumerable<BoardGameId> GetPossessedGameIds(string username);
        IEnumerable<WantToPlayId> GetWantToPlayGameIds(string username);

        IEnumerable<BoardGame> RetrieveBoardGames(int[] ids);
        IEnumerable<Play> RetrievePlays(string username);
    }
}
