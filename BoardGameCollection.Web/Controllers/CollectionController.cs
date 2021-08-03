using BoardGameCollection.Core.Models;
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
        private readonly IBoardGameManager _boardGameManager;

        public CollectionController(IMemoryCache cache, IConfiguration configuration, IBoardGameManager boardGameManager)
        {
            _cache = cache;
            _configuration = configuration;
            _boardGameManager = boardGameManager;
        }

        public ActionResult Owned(string username, [FromQuery] int? bestWith)
        {
            var list = new OwnedGamesList(GetGameList((manager, s) => manager.GetGamePossessions(s), username, bestWith, hideParentlessExpansions: true).ToList());
            return View(list);
        }

        public ActionResult Wishlist(string username, [FromQuery] int? bestWith)
        {
            return View(GetGameList((manager, s) => manager.GetGameWishlist(s), username, bestWith));
        }

        public ActionResult WantToPlay(string username, [FromQuery] int? bestWith)
        {
            return View(GetGameList((manager, s) => manager.GetWantToPlayGames(s), username, bestWith));
        }

        public ActionResult Plays([FromQuery] int? bestWith)
        {
            return View(GetGameList((manager, s) => manager.GetBoardGamePlays(), "kuhlschrank", bestWith));
        }

        private IEnumerable<T> GetGameList<T>(Func<IBoardGameManager, string, IEnumerable<T>> listDelegate, string username, int? bestWith = null, bool hideParentlessExpansions = false) where T : IHasBoardGame
        {
            var list = new List<T>();
            if (string.IsNullOrWhiteSpace(username)) username = "kuhlschrank,cemon,the_happy_llama,Toni_Mahony";
            foreach (var singleName in username.Split(','))
            {
                var cacheKey = $"{singleName}|{listDelegate.GetHashCode()}";
                var allGames = _cache.GetOrCreate(cacheKey, entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    return listDelegate.Invoke(_boardGameManager, singleName);
                }).ToList();
                var gamesToAdd = allGames.ToList().AsEnumerable();
                
                // best-with filter
                if (bestWith.HasValue)
                    gamesToAdd = gamesToAdd.Where(hbg => hbg.BoardGame.IsExpansion || hbg.BoardGame.IsBestWith(bestWith.Value)).ToList();

                if (hideParentlessExpansions)
                {
                    // remove parentless expansions
                    var parentfulExpansionIds = gamesToAdd.Where(bg => !bg.BoardGame.IsExpansion).SelectMany(bg => bg.BoardGame.ExpansionIds).ToList();
                    gamesToAdd = gamesToAdd.Where(bg => !bg.BoardGame.IsExpansion || parentfulExpansionIds.Contains(bg.BoardGame.Id)).ToList();
                }

                list.AddRange(gamesToAdd);
            }
            return list;
        }
    }
}
