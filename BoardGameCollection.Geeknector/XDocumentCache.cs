using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace BoardGameCollection.Geeknector;

public interface IXDocumentCache
{
    Task<XDocument> GetOrAddAsync(
        string key,
        Func<CancellationToken, Task<XDocument>> factory,
        MemoryCacheEntryOptions? options = null,
        CancellationToken cancellationToken = default);

    XDocument GetOrAdd(
        string key,
        Func<XDocument> factory,
        MemoryCacheEntryOptions? options = null);

    void Remove(string key);
}

public sealed class XDocumentCache : IXDocumentCache
{
    private static readonly MemoryCacheEntryOptions DefaultOptions = new MemoryCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromMinutes(3));

    private readonly IMemoryCache _cache;

    // One lock per key: a slow load for key A does not block key B,
    // but concurrent requests for the same key only run the factory once.
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public XDocumentCache(IMemoryCache cache) => _cache = cache;

    public async Task<XDocument> GetOrAddAsync(
        string key,
        Func<CancellationToken, Task<XDocument>> factory,
        MemoryCacheEntryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(factory);

        // Fast path: no locking on cache hits.
        if (_cache.TryGetValue(key, out XDocument? doc) && doc is not null)
            return doc;

        var gate = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // Double-check: another caller may have filled the cache while we waited.
            if (_cache.TryGetValue(key, out doc) && doc is not null)
                return doc;

            doc = await factory(cancellationToken).ConfigureAwait(false)
                  ?? throw new InvalidOperationException($"The factory for key '{key}' returned null.");

            _cache.Set(key, doc, options ?? DefaultOptions);
            return doc;
        }
        finally
        {
            gate.Release();
        }
    }

    public XDocument GetOrAdd(
        string key,
        Func<XDocument> factory,
        MemoryCacheEntryOptions? options = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(factory);

        if (_cache.TryGetValue(key, out XDocument? doc) && doc is not null)
            return doc;

        var gate = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        gate.Wait();
        try
        {
            if (_cache.TryGetValue(key, out doc) && doc is not null)
                return doc;

            doc = factory()
                  ?? throw new InvalidOperationException($"The factory for key '{key}' returned null.");

            _cache.Set(key, doc, options ?? DefaultOptions);
            return doc;
        }
        finally
        {
            gate.Release();
        }
    }

    public void Remove(string key) => _cache.Remove(key);
}