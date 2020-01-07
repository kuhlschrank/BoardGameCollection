using BoardGameCollection.Core.Services;
using System.Collections.Generic;
using System.Linq;
using CoreModels = BoardGameCollection.Core.Models;
using System;
using BoardGameCollection.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BoardGameCollection.Data
{
    public class BoardGameRepository : IBoardGameRepository
    {
        AutoMapper.IMapper _mapper;
        private readonly string _connectionString;

        public BoardGameRepository(IConfiguration configuration)
        {
            _mapper = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BoardGame, CoreModels.BoardGame>()
                .ForMember(d => d.ExpansionIds, o => o.MapFrom(s => s.Expansions.Select(e => e.ExpansionId).ToList()))
                .ForMember(d => d.SuggestedPlayerNumbers, o => o.MapFrom(s => (s.SuggestedPlayerNumbers ?? "")
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)));

                cfg.CreateMap<Play, CoreModels.Play>();
                cfg.CreateMap<PlayPlayer, CoreModels.PlayPlayer>();
            }).CreateMapper();
            _connectionString = configuration.GetConnectionString("CollectionContext");
        }

        public IEnumerable<CoreModels.BoardGame> GetBoardGames(IEnumerable<int> ids)
        {
            using (var db = new CollectionContext(_connectionString))
            {
                var boardGames = db.BoardGames.Include(e => e.Expansions).Where(bg => ids.ToList().Contains(bg.Id)).ToList();
                var models = _mapper.Map<List<BoardGame>, IEnumerable<CoreModels.BoardGame>>(boardGames);
                return models;
            }
        }

        public IEnumerable<CoreModels.BoardGame> GetNextBoardGamesToUpdate(int count, int minimumHoursPassed)
        {
            using (var db = new CollectionContext(_connectionString))
            {
                var minimumDate = DateTimeOffset.Now.AddHours(-minimumHoursPassed);
                var boardGames = db.BoardGames.Where(bg => bg.LastUpdate < minimumDate).OrderBy(bg => bg.LastUpdate).Take(count).ToList();
                var models = _mapper.Map<List<BoardGame>, IEnumerable<CoreModels.BoardGame>>(boardGames);
                return models;
            }
        }

        public void StoreBoardGames(IEnumerable<CoreModels.BoardGame> boardGames)
        {
            using (var db = new CollectionContext(_connectionString))
            {
                foreach (var boardGame in boardGames)
                {
                    var entity = db.BoardGames.Include(e => e.Expansions).SingleOrDefault(bg => bg.Id == boardGame.Id);
                    if (entity == null)
                    {
                        entity = new BoardGame() { Id = boardGame.Id };
                        db.BoardGames.Add(entity);
                    }
                    entity.Title = boardGame.Title;
                    entity.ThumbnailUri = boardGame.ThumbnailUri;
                    entity.ImageUri = boardGame.ImageUri;
                    entity.MinPlayers = boardGame.MinPlayers;
                    entity.MaxPlayers = boardGame.MaxPlayers;
                    entity.YearPublished = boardGame.YearPublished;
                    entity.SuggestedPlayerNumbers = string.Join(";", boardGame.SuggestedPlayerNumbers);
                    entity.AverageRating = boardGame.AverageRating;
                    entity.LastUpdate = DateTimeOffset.Now;
                    entity.IsExpansion = boardGame.IsExpansion;

                    entity.Expansions.Clear();
                    foreach (var expansionId in boardGame.ExpansionIds)
                        entity.Expansions.Add(new Expansion { ExpansionId = expansionId });
                }
                db.SaveChanges();
            }
        }

        public IEnumerable<CoreModels.Play> GetAllPlays()
        {
            using (var db = new CollectionContext(_connectionString))
            {
                var plays = db.Plays.Include(e => e.Players).ToList();
                var models = _mapper.Map<List<Play>, IEnumerable<CoreModels.Play>>(plays);
                return models;
            }
        }

        public void StorePlays(IEnumerable<CoreModels.Play> plays)
        {
            using (var db = new CollectionContext(_connectionString))
            {
                var count = 0;
                foreach (var play in plays)
                {
                    var entity = db.Plays.Include(e => e.Players).SingleOrDefault(p => p.Id == play.Id);
                    if (entity == null)
                    {
                        entity = new Play() { Id = play.Id };
                        db.Plays.Add(entity);
                    }

                    entity.BoardGameId = play.BoardGameId;
                    entity.Comments = play.Comments;
                    entity.Date = play.Date;
                    entity.IgnoreForStatistics = play.IgnoreForStatistics;
                    entity.Location = play.Location;
                    entity.Quantity = play.Quantity;

                    entity.Players.Clear();
                    foreach (var player in play.Players)
                        entity.Players.Add(new PlayPlayer
                        {
                            Position = player.Position,
                            Name = player.Name,
                            Username = player.Username,
                            UserId = player.UserId,
                            Score = player.Score,
                            Color = player.Color,
                            New = player.New,
                            Win = player.Win
                        });
                    if (++count >= 25)
                    {
                        db.SaveChanges();
                        count = 0;
                    }
                }
                db.SaveChanges();
            }
        }

        public void StoreUnknownIds(IEnumerable<int> unknownIds)
        {
            using (var db = new CollectionContext(_connectionString))
            {
                foreach (var id in unknownIds)
                {
                    if (!db.BoardGames.Any(bg => bg.Id == id) && !db.Unknowns.Any(bg => bg.Id == id))
                    {
                        db.Unknowns.Add(new Unknown { Id = id });
                        db.SaveChanges();
                    }
                }
            }
        }

        public int[] GetUnknownIds(int count)
        {
            using (var db = new CollectionContext(_connectionString))
            {
                return db.Unknowns.Take(count).Select(un => un.Id).ToArray();
            }
        }

        public void DeleteUnknownIds(IEnumerable<int> ids)
        {
            using (var db = new CollectionContext(_connectionString))
            {
                db.Unknowns.RemoveRange(db.Unknowns.Where(un => ids.Contains(un.Id)));
                db.SaveChanges();
            }
        }
    }
}
