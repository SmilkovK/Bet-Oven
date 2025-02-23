using SportDomain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportRepository.Interface
{
    public interface IUserRepository
    {
        IEnumerable<BetUser> GetAll();
        BetUser Get(string? id);
        void Insert(BetUser entity);
        void Update(BetUser entity);
        void Delete(BetUser entity);
    }
}
