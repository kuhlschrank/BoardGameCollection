using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameCollection.Api.Models;
using BoardGameCollection.Core.Services;
using BoardGameCollection.Data;
using BoardGameCollection.Domain;
using BoardGameCollection.Geeknector;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameCollection.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CollectionController : ControllerBase
    {
        public object Owned(string username)
        {
            return new OwnedGamesList(GetGameList((manager, s) => manager.GetGamePossessions(s), username).ToList());
        }

        public object Wishlist(string username)
        {
            return GetGameList((manager, s) => manager.GetGameWishlist(s), username);
        }

        public object WantToPlay(string username)
        {
            return GetGameList((manager, s) => manager.GetWantToPlayGames(s), username);
        }

        private IEnumerable<T> GetGameList<T>(Func<IBoardGameManager, string, IEnumerable<T>> listDelegate, string username) where T : class
        {
            var list = new List<T>();
            if (string.IsNullOrWhiteSpace(username)) username = "kuhlschrank,cemon,the_happy_llama";
            IBoardGameManager c = new BoardGameManager(new GeekConnector(), new BoardGameRepository(), new BoardGameCrawler(new GeekConnector(), new BoardGameRepository()));
            foreach (var singleName in username.Split(','))
            {
                var cacheKey = $"{singleName}|{listDelegate.GetHashCode()}";
                var gamesList = listDelegate(c, singleName);
                //var gamesList = HttpRuntime.Cache.Get(cacheKey) as IEnumerable<T>;
                //if (gamesList == null)
                //{
                //    gamesList = listDelegate(c, singleName);
                //    HttpRuntime.Cache.Insert(cacheKey, gamesList, null, DateTime.UtcNow.AddMinutes(5), Cache.NoSlidingExpiration);
                //}
                list.AddRange(gamesList);
            }
            return list;
        }
    }
}
