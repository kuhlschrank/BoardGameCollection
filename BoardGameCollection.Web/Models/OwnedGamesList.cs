using BoardGameCollection.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGameCollection.Web.Models
{
    public class OwnedGamesList : List<OwnedGame>
    {
        public List<string> Owners { get; set; }

        private readonly IEqualityComparer<BoardGame> _boardGameComparer = new BoardGameEqualityComparer();

        class BoardGameEqualityComparer : IEqualityComparer<BoardGame>
        {
            public bool Equals(BoardGame x, BoardGame y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(BoardGame obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        public OwnedGamesList(List<GamePossession> possessions)
        {
            var allGames = possessions.Select(p => p.BoardGame).ToList().Distinct(_boardGameComparer).ToList();
            var allExpansionIds = allGames.SelectMany(bg => bg.ExpansionIds).Distinct().ToList();

            var baseGames = allGames.Where(bg => !allExpansionIds.Contains(bg.Id));

            foreach (var baseGame in baseGames)
            {
                var ownedGame = new OwnedGame
                {
                    BoardGame = baseGame,
                    Owners = possessions.Where(p => p.BoardGame.Id == baseGame.Id).Select(p => p.Owner).Distinct().ToList(),
                    Expansions = new List<OwnedExpansion>()
                };
                foreach (var expansion in FindAllTransitiveExpansions(baseGame, allGames).Distinct(_boardGameComparer))
                {
                    ownedGame.Expansions.Add(new OwnedExpansion
                    {
                        BoardGame = expansion,
                        Owners = possessions.Where(p => p.BoardGame.Id == expansion.Id).Select(p => p.Owner).Distinct().ToList()
                    });
                }
                Add(ownedGame);
            }

            Owners = possessions.Select(p => p.Owner).Distinct().ToList();
        }

        private static IEnumerable<BoardGame> FindAllTransitiveExpansions(BoardGame baseGame, List<BoardGame> allGames)
        {
            foreach (var expansionId in baseGame.ExpansionIds)
            {
                var expansion = allGames.FirstOrDefault(bg => bg.Id == expansionId);
                if (expansion != null)
                {
                    yield return expansion;
                    foreach (var game in FindAllTransitiveExpansions(expansion, allGames))
                        yield return game;
                }
            }
        }
    }

    public class OwnedGame
    {
        public BoardGame BoardGame { get; set; }
        public List<OwnedExpansion> Expansions { get; set; }
        public List<string> Owners { get; set; }
    }

    public class OwnedExpansion
    {
        public BoardGame BoardGame { get; set; }
        public List<string> Owners { get; set; }
    }
}