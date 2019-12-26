using BoardGameCollection.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCollection.Web.Views
{
    public static class CoreModelExtensions
    {
        public static string BestPlayerCountTag(this BoardGame boardGame)
        {
            if (!boardGame.BestPlayerNumbers.Any())
                return "&nbsp;";

            var numbers = boardGame.BestPlayerNumbers
                .Select(n => new { num = Int32.Parse(n.TrimEnd('+')), andSoOn = n.EndsWith('+'), original = n });
            var orderedNumbers = numbers
                .Where(n => n.andSoOn || !numbers.Any(other => other.num == n.num && other.andSoOn))
                .OrderBy(n => n.num).ThenBy(n => n.andSoOn)
                .ToList();
            var groups = new List<(string from, string to)>();
            var currentGroupFrom = orderedNumbers.First();
            var currentGroupTo = orderedNumbers.First();
            foreach (var number in orderedNumbers)
            {
                if (currentGroupFrom == number) continue;

                // extend group
                if (currentGroupTo.num == number.num - 1) currentGroupTo = number;

                // close group, if any
                if (currentGroupTo.num < number.num - 1)
                {
                    groups.Add((currentGroupFrom.original, currentGroupTo.original));
                    currentGroupFrom = number;
                    currentGroupTo = number;
                }
            }
            // close last group
            groups.Add((currentGroupFrom.original, currentGroupTo.original));
            var groupStrings = groups.Select(g => g.from != g.to ? $"{g.from} - {g.to}" : g.from);

            return string.Join("|", string.Join(", ", groupStrings));
        }

        public static string NotRecommendedPlayerCountTag(this BoardGame boardGame)
        {
            return "&nbsp;";
        }
    }
}
