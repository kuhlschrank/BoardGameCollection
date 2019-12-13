using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BoardGameCollection.Crawler
{
    public class CrawlService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                while (true)
                {
                    await ExecuteAsync();
                    await Task.Delay(5000, cancellationToken);
                }
            });
        }

        public async Task ExecuteAsync()
        {
            Console.Write("Crawling...");
            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
