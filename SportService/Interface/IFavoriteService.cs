using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace SportService.Interface
{
    public interface IFavoriteService
    {
        List<int> GetFavoriteLeagues(string userId);
        void AddFavoriteLeague(string userId, int leagueId);
        void RemoveFavoriteLeague(string userId, int leagueId);
        bool IsFavorite(string userId, int leagueId);
    }
}