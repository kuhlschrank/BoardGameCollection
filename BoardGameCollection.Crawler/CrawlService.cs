using BoardGameCollection.Core.Services;
using BoardGameCollection.Data;
using BoardGameCollection.Geeknector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BoardGameCollection.Crawler
{
    public class CrawlService : IHostedService
    {
        private readonly IBoardGameRepository _repository;
        private readonly IGeekConnector _geekConnector;

        public CrawlService(IBoardGameRepository repository, IGeekConnector geekConnector)
        {
            _repository = repository;
            _geekConnector = geekConnector;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            TimeSpan inverval = TimeSpan.FromMinutes(1);
            return Task.Run(async () =>
            {
                while (true)
                {
                    await ExecuteAsync();
                    await Task.Delay(inverval, cancellationToken);
                }
            });
        }

        public async Task ExecuteAsync()
        {
            var res = _geekConnector.RetrievePlays("kuhlschrank");
            //CrawlOnce();
            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void CrawlOnce()
        {
            CrawlOnceUnknown(500);
            CrawlOnceKnown(20, 48);
        }

        private void CrawlOnceUnknown(int maxCount)
        {
            var ids = _repository.GetUnknownIds(maxCount);
            if (ids.Any())
            {
                Log($"{ids.Count()} unknown boardgames.");
                var boardGames = _geekConnector.RetrieveBoardGames(ids).ToList();
                Log($"{boardGames.Count()} boardgames retrieved.");
                _repository.StoreBoardGames(boardGames);
                _repository.DeleteUnknownIds(ids);
            }
        }

        private void CrawlOnceKnown(int maxCount, int minimumHoursPassed)
        {
            var ids = _repository.GetNextBoardGamesToUpdate(maxCount, minimumHoursPassed).Select(bg => bg.Id).ToArray();
            if (ids.Any())
            {
                Log($"{ids.Count()} known boardgames.");
                var boardGames = _geekConnector.RetrieveBoardGames(ids).ToList();
                Log($"{boardGames.Count()} boardgames updated.");
                _repository.StoreBoardGames(boardGames);
            }
        }

        static void Log(object message)
        {
#if DEBUG
            System.Console.WriteLine("{0} - {1}", DateTime.Now, message);
#endif
        }
    }
}
