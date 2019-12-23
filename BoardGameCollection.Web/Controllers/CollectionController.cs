using BoardGameCollection.Core.Services;
using BoardGameCollection.Data;
using BoardGameCollection.Domain;
using BoardGameCollection.Geeknector;
using BoardGameCollection.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGameCollection.Web.Controllers
{
    public class CollectionController : Controller
    {
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
                //var gamesList = HttpRuntime.Cache.Get(cacheKey) as IEnumerable<T>;
                //if (gamesList == null)
                //{
                    var gamesList = listDelegate(c, singleName);
                //    HttpRuntime.Cache.Insert(cacheKey, gamesList, null, DateTime.UtcNow.AddMinutes(5), Cache.NoSlidingExpiration);
                //}
                list.AddRange(gamesList);
            }
            return list;
        }
    }
}
