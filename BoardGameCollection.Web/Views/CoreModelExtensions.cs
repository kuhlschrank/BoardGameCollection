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
            if (boardGame == null || boardGame.SuggestedPlayerNumbers == null)
                return "&nbsp;";

            return SuggestedPlayerCountTag(
                boardGame.SuggestedPlayerNumbers.Where(n => !n.StartsWith('-')));
            
        }

        public static string NotRecommendedPlayerCountTag(this BoardGame boardGame)
        {
            if (boardGame == null || boardGame.SuggestedPlayerNumbers == null)
                return "&nbsp;";

            return SuggestedPlayerCountTag(
                boardGame.SuggestedPlayerNumbers
                    .Where(n => n.StartsWith('-'))
                    .Select(n => n.TrimStart('-')));
        }

        private static string SuggestedPlayerCountTag(IEnumerable<string> sanitizedPlayerNumbers)
        {
            if (!sanitizedPlayerNumbers.Any())
                return "&nbsp;";

            var numbers = sanitizedPlayerNumbers
                .Select(n => new { num = Int32.Parse(n.TrimEnd('+')), andSoOn = n.EndsWith('+') })
                .ToList();

            // for BGG "8+" means "more than 8"... not "8 or more"
            var correctedNumbers = numbers
                .Select(n => new { num = n.num, andSoOn = n.andSoOn, display = n.andSoOn ? $"{n.num + 1}+" : $"{n.num}" })
                .ToList();

            var orderedNumbers = correctedNumbers
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
                    groups.Add((currentGroupFrom.display, currentGroupTo.display));
                    currentGroupFrom = number;
                    currentGroupTo = number;
                }
            }
            // close last group
            groups.Add((currentGroupFrom.display, currentGroupTo.display));
            var groupStrings = groups.Select(g => g.from != g.to ? $"{g.from}-{g.to}" : g.from);

            return string.Join(", ", groupStrings);
        }
    }
}
