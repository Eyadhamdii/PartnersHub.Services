using PartnersHub.Synergy.Domain.Aggregates.Synergy.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Synergy.Application.Interfaces.Repository
{
    public interface IThematicAreaRepository
    {
        Task<IEnumerable<ThematicArea>> GetAllAsync();
        Task<bool> ExistsAsync(int id);
        Task<ThematicArea> GetById(int id);

    }
}
