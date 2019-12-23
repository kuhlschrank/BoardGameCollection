using BoardGameCollection.Core.Services;
using BoardGameCollection.Data;
using BoardGameCollection.Domain;
using BoardGameCollection.Geeknector;
using BoardGameCollection.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGameCollection.Web.Controllers
{
    public class CollectionController : Controller
    {
        private IMemoryCache _cache;

        public CollectionController(IMemoryCache cache)
        {
            _cache = cache;
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
            IBoardGameManager c = new BoardGameManager(new GeekConnector(), new BoardGameRepository(), new BoardGameCrawler(new GeekConnector(), new BoardGameRepository()));
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
