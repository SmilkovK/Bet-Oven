using SportDomain.models;
using SportRepository;
using SportService.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportService.Implementation
{
    public class FavoriteService : IFavoriteService
    {
        private readonly ApplicationDbContext _context;

        public FavoriteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<int> GetFavoriteLeagues(string userId)
        {
            return _context.FavoriteLeagues
                .Where(fl => fl.UserId == userId)
                .Select(fl => fl.LeagueId)
                .ToList();
        }

        public void AddFavoriteLeague(string userId, int leagueId)
        {
            if (!_context.FavoriteLeagues.Any(fl => fl.UserId == userId && fl.LeagueId == leagueId))
            {
                _context.FavoriteLeagues.Add(new FavoriteLeague
                {
                    UserId = userId,
                    LeagueId = leagueId
                });
                _context.SaveChanges();
            }
        }

        public void RemoveFavoriteLeague(string userId, int leagueId)
        {
            var favorite = _context.FavoriteLeagues
                .FirstOrDefault(fl => fl.UserId == userId && fl.LeagueId == leagueId);

            if (favorite != null)
            {
                _context.FavoriteLeagues.Remove(favorite);
                _context.SaveChanges();
            }
        }

        public bool IsFavorite(string userId, int leagueId)
        {
            return _context.FavoriteLeagues
                .Any(fl => fl.UserId == userId && fl.LeagueId == leagueId);
        }
    }
}
