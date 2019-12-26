using BoardGameCollection.Core.Services;
using BoardGameCollection.Data;
using BoardGameCollection.Domain;
using BoardGameCollection.Geeknector;
using BoardGameCollection.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGameCollection.Web.Controllers
{
    public class CollectionController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public CollectionController(IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
        }

        public ActionResult Owned(string username)
        {
            var list = new OwnedGamesList(GetGameList((manager, s) => manager.GetGamePossessions(s), username).ToList());
            return View(list);
        }

        public ActionResult Wishlist(string username)
        {
            return View(GetGameList((manager, s) => manager.GetGameWishlist(s), username));
        }

        public ActionResult WantToPlay(string username)
        {
            return View(GetGameList((manager, s) => manager.GetWantToPlayGames(s), username));
        }

        private IEnumerable<T> GetGameList<T>(Func<IBoardGameManager, string, IEnumerable<T>> listDelegate, string username) where T : class
        {
            var list = new List<T>();
            if (string.IsNullOrWhiteSpace(username)) username = "kuhlschrank,cemon,the_happy_llama";
            var repo = new BoardGameRepository(_configuration.GetConnectionString("CollectionContext"));
            var nector = new GeekConnector();
            IBoardGameManager c = new BoardGameManager(nector, repo , new BoardGameCrawler(nector, repo));
            foreach (var singleName in username.Split(','))
            {
                var cacheKey = $"{singleName}|{listDelegate.GetHashCode()}";
                var gamesList = _cache.GetOrCreate(cacheKey, entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    return listDelegate.Invoke(c, singleName);
                });
                list.AddRange(gamesList);
            }
            return list;
        }
    }
}
