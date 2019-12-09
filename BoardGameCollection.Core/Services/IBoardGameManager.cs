using BoardGameCollection.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameCollection.Core.Services
{
    public interface IBoardGameManager
    {
        IEnumerable<GamePossession> GetGamePossessions(string username);
        IEnumerable<GameWish> GetGameWishlist(string username);
        IEnumerable<WantToPlayGame> GetWantToPlayGames(string username);
    }
}