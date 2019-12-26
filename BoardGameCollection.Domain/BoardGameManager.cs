using BoardGameCollection.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoardGameCollection.Core.Models;
using BoardGameCollection.Core.GeekConnector.Models;

namespace BoardGameCollection.Domain
{
    public class BoardGameManager : IBoardGameManager
    {
        private readonly IGeekConnector _geekConnector;
        private readonly IBoardGameRepository _boardGameRepository;

        public BoardGameManager(IGeekConnector geekConnector, IBoardGameRepository boardGameRepository)
        {
            _geekConnector = geekConnector;
            _boardGameRepository = boardGameRepository;
        }

        public IEnumerable<GamePossession> GetGamePossessions(string username)
        {
            var ids = _geekConnector.GetPossessedGameIds(username).ToList();
            var games = _boardGameRepository.GetBoardGames(ids.Select(i => i.Id)).ToList();

            var missingIds = ids.Where(id => !games.Select(g => g.Id).Contains(id.Id)).ToList();
            games.AddRange(missingIds.Select(CreateDummyGame));

            _boardGameRepository.StoreUnknownIds(missingIds.Select(id => id.Id));

            return ids.Select(id => new GamePossession
            {
                BoardGame = games.First(g => g.Id == id.Id),
                Owner = username
            });
        }

        public IEnumerable<GameWish> GetGameWishlist(string username)
        {
            var ids = _geekConnector.GetWishlistGameIds(username).ToList();
            var games = _boardGameRepository.GetBoardGames(ids.Select(i => i.Id)).ToList();

            var missingIds = ids.Where(id => !games.Select(g => g.Id).Contains(id.Id)).ToList();
            games.AddRange(missingIds.Select(CreateDummyGame));

            _boardGameRepository.StoreUnknownIds(missingIds.Select(id => id.Id));

            return ids.Select(id => new GameWish
            {
                BoardGame = games.First(g => g.Id == id.Id),
                Owner = username,
                Priority = id.Priority,
                Comment = id.Comment
            });
        }

        public IEnumerable<WantToPlayGame> GetWantToPlayGames(string username)
        {
            var ids = _geekConnector.GetWantToPlayGameIds(username).ToList();
            var games = _boardGameRepository.GetBoardGames(ids.Select(i => i.Id)).ToList();

            var missingIds = ids.Where(id => !games.Select(g => g.Id).Contains(id.Id)).ToList();
            games.AddRange(missingIds.Select(CreateDummyGame));

            _boardGameRepository.StoreUnknownIds(missingIds.Select(id => id.Id));

            return ids.Select(id => new WantToPlayGame
            {
                BoardGame = games.First(g => g.Id == id.Id),
                Owner = username,
                LastModified = id.LastModified
            });
        }

        private BoardGame CreateDummyGame(BoardGameId id)
        {
            return new BoardGame
            {
                Id = id.Id,
                Title = id.Name
            };
        }
    }
}
