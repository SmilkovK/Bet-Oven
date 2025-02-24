using SportDomain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportService.Interface
{
    public interface IVirtualCurrencyService
    {
        List<VirtualCurrency> GetAllVCurrency();
        VirtualCurrency GetDetailsForVCurrency(Guid? id);
        void CreateNewVCurrency(VirtualCurrency p);
        void UpdateExistingVCurrency(VirtualCurrency p);
        void DeleteVCurrency(Guid id);
    }
}
