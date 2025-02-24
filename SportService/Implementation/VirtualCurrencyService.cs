using Microsoft.EntityFrameworkCore.Migrations;
using SportDomain.models;
using SportRepository.Interface;
using SportService.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportService.Implementation
{
    public class VirtualCurrencyService : IVirtualCurrencyService
    {
        private readonly IRepository<VirtualCurrency> _vcurrency;
   
        private readonly IUserRepository _userRepository;

        public VirtualCurrencyService(IRepository<VirtualCurrency> vcurrencyRepository, IUserRepository userRepository)
        {
            _vcurrency = vcurrencyRepository;
            _userRepository = userRepository;
        }

        public void CreateNewVCurrency(VirtualCurrency p)
        {
            _vcurrency.Insert(p);
        }

        public void DeleteVCurrency(Guid id)
        {
            var vcurreny = _vcurrency.Get(id);
            _vcurrency.Delete(vcurreny);
        }

        public List<VirtualCurrency> GetAllVCurrency()
        {
            return _vcurrency.GetAll().ToList();
        }

        public VirtualCurrency GetDetailsForVCurrency(Guid? id)
        {
            var vcurreny = _vcurrency.Get(id);
            return vcurreny;
        }

        public void UpdateExistingVCurrency(VirtualCurrency p)
        {
            _vcurrency.Update(p);
        }
    }
}

