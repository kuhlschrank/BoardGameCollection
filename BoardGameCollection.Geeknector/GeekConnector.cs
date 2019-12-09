using System.Globalization;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using BoardGameCollection.Core.GeekConnector.Models;
using BoardGameCollection.Core.Services;
using BoardGameCollection.Core.Models;
using System.Threading;

namespace BoardGameCollection.Geeknector
{
    public class GeekConnector : IGeekConnector
    {
        //<item collid="19005529" subtype="boardgame" objectid="105551" objecttype="thing">
        //    <name sortindex="1">Archipelago</name>
        //    <status lastmodified="2013-04-22 17:04:29" preordered="0" wishlist="1" wanttobuy="0" wanttoplay="0" want="0" fortrade="0" prevowned="0" own="0" wishlistpriority="2"/>
        //</item>

        public IEnumerable<WishlistId> GetWishlistGameIds(string username)
        {
            var doc = RetrieveGeekCollection(username, "wishlist");
            foreach (var child in doc.Root.Elements())
            {
                var id = Int32.Parse(child.Attribute("objectid").Value);
                var name = child.Element("name").Value;
                var priority = byte.Parse(child.Element("status").Attribute("wishlistpriority").Value);
                var comment = child.Element("wishlistcomment")?.Value;
                yield return new WishlistId { Id = id, Name = name, Priority = priority, Comment = comment };
            }
        }

        public IEnumerable<BoardGameId> GetPossessedGameIds(string username)
        {
            var doc = RetrieveGeekCollection(username, "own");
            foreach (var child in doc.Root.Elements())
            {
                var id = Int32.Parse(child.Attribute("objectid").Value);
                var name = child.Element("name").Value;
                yield return new BoardGameId { Id = id, Name = name };
            }
        }

        public IEnumerable<WantToPlayId> GetWantToPlayGameIds(string username)
        {
            var doc = RetrieveGeekCollection(username, "wanttoplay");
            foreach (var child in doc.Root.Elements())
            {
                var id = Int32.Parse(child.Attribute("objectid").Value);
                var name = child.Element("name").Value;
                var lastModified = DateTime.Parse(child.Element("status").Attribute("lastmodified").Value);
                yield return new WantToPlayId { Id = id, Name = name, LastModified = lastModified };
            }
        }

        private XDocument RetrieveGeekCollection(string username, string listType)
        {
            var brief = listType == "wishlist" ? 0 : 1;
            var tries = 3; // der Geek liefert manchmal für eine Anfrage keine Daten...
            while (true)
            {
                var request = WebRequest.Create(string.Format("http://boardgamegeek.com/xmlapi2/collection?brief={2}&username={0}&{1}=1", username, listType, brief));
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK && tries > 0)
                {
                    Thread.Sleep(333);
                    continue;
                }
                string responseString;
                using (var reader = new StreamReader(response.GetResponseStream()))
                    responseString = reader.ReadToEnd();
                return XDocument.Parse(responseString);
            }
        }



        public IEnumerable<BoardGame> RetrieveBoardGames(int[] ids)
        {
            int pageSize = 10;
            var factory = new TaskFactory<List<BoardGame>>();
            var noTasks = ids.Length / pageSize + ((ids.Length % pageSize) > 0 ? 1 : 0);
            var tasks = new Task<List<BoardGame>>[noTasks];

            for (var i = 0; i < noTasks; i++)
            {
                var page = ids.Skip(i * pageSize).Take(pageSize);
                var task = factory.StartNew(() =>
                    {
                        try
                        {
                            var boardGames = new List<BoardGame>();
                            var request =
                                WebRequest.Create("http://boardgamegeek.com/xmlapi2/thing?stats=1&id=" +
                                                  string.Join(",", page));
                            var response = request.GetResponse();
                            string responseString;
                            using (var reader = new StreamReader(response.GetResponseStream()))
                                responseString = reader.ReadToEnd();
                            XDocument doc = XDocument.Parse(responseString);
                            XElement root = doc.Root;

                            var items = (from child in root.Elements() select child).ToList();
                            boardGames.AddRange(items.Select(ParseBoardGameThing));

                            return boardGames;
                        }
                        catch (WebException ex)
                        {
                            return page.Select(id => new BoardGame() { Id = id, Title = ex.Message }).ToList();
                        }
                    });
                tasks[i] = task;
            }
            Task.WaitAll(tasks);

            return tasks.SelectMany(task => task.Result).ToList();

        }

        private BoardGame ParseBoardGameThing(XElement node)
        {
            var idNode = node.Attribute(XName.Get("id"));
            var typeNode = node.Attribute(XName.Get("type"));
            var titleNode = node.Elements(XName.Get("name")).FirstOrDefault(x => x.Attribute(XName.Get("type")).Value == "primary").Attribute(XName.Get("value"));
            var minPlayersNode = node.Elements(XName.Get("minplayers")).FirstOrDefault().Attribute(XName.Get("value"));
            var maxPlayersNode = node.Elements(XName.Get("maxplayers")).FirstOrDefault().Attribute(XName.Get("value"));
            var thumnailUriNode = node.Elements(XName.Get("thumbnail")).FirstOrDefault();
            var imageUriNode = node.Elements(XName.Get("image")).FirstOrDefault();
            var yearPublishedNode = node.Elements(XName.Get("yearpublished")).FirstOrDefault().Attribute(XName.Get("value"));
            var bestPlayerNunmberNode = node.Elements(XName.Get("poll")).FirstOrDefault(x => x.Attribute(XName.Get("name")).Value == "suggested_numplayers");

            var statisticsNode = node.Elements(XName.Get("statistics")).FirstOrDefault();
            var ratingsNode = statisticsNode.Elements(XName.Get("ratings")).FirstOrDefault();
            var averageAttribute = ratingsNode.Elements(XName.Get("average")).FirstOrDefault().Attribute(XName.Get("value"));

            var expansionIds = new List<int>();

            var linkNodes = node.Elements(XName.Get("link")).Where(x => x.Attribute("type").Value == "boardgameexpansion" && x.Attribute("inbound") == null);
            expansionIds.AddRange(linkNodes.Select(linkNode => Int32.Parse(linkNode.Attribute(XName.Get("id")).Value)));

            var bestPlayerNumber = EstimateBestPlayerNumber(bestPlayerNunmberNode);
            var boardGame = new BoardGame
            {
                Id = idNode == null ? 0 : Int32.Parse(idNode.Value),
                Title = titleNode == null ? "" : titleNode.Value,
                MinPlayers = minPlayersNode == null ? -1 : Int32.Parse(minPlayersNode.Value),
                MaxPlayers = maxPlayersNode == null ? 99 : Int32.Parse(maxPlayersNode.Value),
                ExpansionIds = expansionIds,
                BestPlayerNumber = bestPlayerNumber,
                ThumbnailUri = thumnailUriNode == null ? "" : thumnailUriNode.Value,
                ImageUri = imageUriNode == null ? "" : imageUriNode.Value,
                YearPublished = yearPublishedNode == null ? 1900 : Int32.Parse(yearPublishedNode.Value),
                AverageRating = averageAttribute == null ? 0 : Double.Parse(averageAttribute.Value, CultureInfo.InvariantCulture)
            };

            return boardGame;
        }

        private static int EstimateBestPlayerNumber(XElement bestPlayerNunmberNode)
        {
            var bestPlayerNumber = 0;
            var bestVotes = 0;
            foreach (var resultsNode in bestPlayerNunmberNode.Elements("results"))
            {
                var numPlayers = Int32.Parse(resultsNode.Attribute("numplayers").Value.Trim('+'));
                var votes = 0;
                if (resultsNode.Elements("result").Where(n => n.Attribute("value").Value == "Best").FirstOrDefault() != null)
                    votes = Int32.Parse(resultsNode.Elements("result").Where(n => n.Attribute("value").Value == "Best").FirstOrDefault().Attribute("numvotes").Value);
                if (votes > bestVotes)
                {
                    bestVotes = votes;
                    bestPlayerNumber = numPlayers;
                }
            }
            return bestPlayerNumber;
        }
    }
}