using BoardGameCollection.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoardGameCollection.Core.GeekConnector.Models;
using System.Threading;
using System.Collections.Concurrent;

namespace BoardGameCollection.Domain
{
    public class BoardGameCrawler : IBoardGameCrawler
    {
        private readonly IGeekConnector _geekConnector;
        private readonly IBoardGameRepository _boardGameRepository;

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private BlockingCollection<BoardGameId> _idsToDo;

        public BoardGameCrawler(IGeekConnector geekConnector, IBoardGameRepository boardGameRepository)
        {
            _geekConnector = geekConnector;
            _boardGameRepository = boardGameRepository;

            _idsToDo = new BlockingCollection<BoardGameId>();

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            var taskFactory = new TaskFactory(_cancellationToken);
            taskFactory.StartNew(CollectBoardGames, _cancellationToken);
        }

        private void CollectBoardGames()
        {
            System.Diagnostics.Debug.WriteLine("CollectBoardGames started.");
            while (!_cancellationToken.IsCancellationRequested)
            {
                System.Diagnostics.Debug.WriteLine("CollectBoardGames loop entered.");
                var callIds = _idsToDo.GetConsumingEnumerable().Take(75).ToList();
                System.Diagnostics.Debug.WriteLine($"CollectBoardGames retrieved { callIds.Count } Ids.");
                var existingGames = _boardGameRepository.GetBoardGames(callIds.Select(id => id.Id)).ToList();
                System.Diagnostics.Debug.WriteLine($"CollectBoardGames skips { existingGames.Count } Ids.");
                callIds = callIds.Where(id => !existingGames.Select(eg => eg.Id).ToList().Contains(id.Id)).ToList();
                System.Diagnostics.Debug.WriteLine($"CollectBoardGames retrieved { callIds.Count } Ids.");
                var games = _geekConnector.RetrieveBoardGames(callIds.Select(i => i.Id).Distinct().ToArray());
                System.Diagnostics.Debug.WriteLine($"CollectBoardGames retrieved { callIds.Count } Games.");
                _boardGameRepository.StoreBoardGames(games);
                System.Diagnostics.Debug.WriteLine($"CollectBoardGames saved { callIds.Count } Games.");
            }
        }

        public void AddUnknownBoardGameIds(IEnumerable<BoardGameId> ids)
        {
            foreach (var id in ids)
                _idsToDo.Add(id);
        }
    }
}
