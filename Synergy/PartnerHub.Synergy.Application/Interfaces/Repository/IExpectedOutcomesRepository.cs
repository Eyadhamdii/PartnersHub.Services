using PartnersHub.Synergy.Domain.Aggregates;
using PartnersHub.Synergy.Domain.Aggregates.Synergy.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Synergy.Application.Interfaces.Repository
{
    public interface IExpectedOutcomesRepository
    {
        Task<List<ExpectedOutcome>> GetAllAsync();
        Task<List<ExpectedOutcome>> GetByIdsAsync(List<int> Ids);

    }
}
