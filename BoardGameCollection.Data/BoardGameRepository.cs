using BoardGameCollection.Core.Services;
using System.Collections.Generic;
using System.Linq;
using CoreModels = BoardGameCollection.Core.Models;
using System;
using BoardGameCollection.Data.Entities;

namespace BoardGameCollection.Data
{
    public class BoardGameRepository : IBoardGameRepository
    {
        AutoMapper.IMapper _mapper;

        public BoardGameRepository()
        {
            _mapper = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BoardGame, CoreModels.BoardGame>()
                .ForMember(d => d.ExpansionIds, o => o.MapFrom(s => s.Expansions.Select(e => e.ExpansionId).ToList()))
                .ReverseMap();
            }).CreateMapper();
        }

        public IEnumerable<CoreModels.BoardGame> GetBoardGames(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
            //using (var db = new BoardGameCollectionEntities())
            //{
            //    var boardGames = db.BoardGames.Where(bg => ids.ToList().Contains(bg.Id)).ToList();
            //    var models = _mapper.Map<List<BoardGame>, IEnumerable<CoreModels.BoardGame>>(boardGames);
            //    return models;
            //}
        }

        public IEnumerable<CoreModels.BoardGame> GetNextBoardGamesToUpdate(int count, int minimumHoursPassed)
        {
            throw new NotImplementedException();
            //using (var db = new BoardGameCollectionEntities())
            //{
            //    var minimumDate = DateTime.Now.AddHours(minimumHoursPassed);
            //    var boardGames = db.BoardGames.Where(bg => bg.LastUpdate < minimumDate).OrderBy(bg => bg.LastUpdate).Take(count).ToList();
            //    var models = _mapper.Map<List<BoardGame>, IEnumerable<CoreModels.BoardGame>>(boardGames);
            //    return models;
            //}
        }

        public void StoreBoardGames(IEnumerable<CoreModels.BoardGame> boardGames)
        {
            using (var db = new MyContext())
            {
                foreach (var boardGame in boardGames)
                {
                    var entity = db.BoardGames.SingleOrDefault(bg => bg.Id == boardGame.Id);
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
                    entity.BestPlayerNumber = boardGame.BestPlayerNumber;
                    entity.AverageRating = boardGame.AverageRating;
                    entity.LastUpdate = DateTime.Now;

                    entity.Expansions.Clear();
                    //foreach (var expansionId in boardGame.ExpansionIds)
                    //    entity.Expansions.Add(new Expansion { ExpansionId = expansionId });
                }

                db.SaveChanges();
            }
        }

        public void StoreUnknownIds(IEnumerable<int> unknownIds)
        {
            throw new NotImplementedException();
            //using (var db = new BoardGameCollectionEntities())
            //{
            //    foreach (var id in unknownIds)
            //    {
            //        if (!db.BoardGames.Any(bg => bg.Id == id) && !db.Unknowns.Any(bg => bg.Id == id))
            //        {
            //            db.Unknowns.Add(new Unknown { Id = id });
            //            db.SaveChanges();
            //        }
            //    }
            //}
        }

        public int[] GetUnknownIds(int count)
        {
            using (var db = new MyContext())
            {
                return db.Unknowns.Take(count).Select(un => un.Id).ToArray();
            }
        }

        public void DeleteUnknownIds(IEnumerable<int> ids)
        {
            using (var db = new MyContext())
            {
                db.Unknowns.RemoveRange(db.Unknowns.Where(un => ids.Contains(un.Id)));
                db.SaveChanges();
            }
        }
    }
}
