using PartnersHub.Synergy.Domain.Aggregates.Synergy.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Synergy.Application.Interfaces.Common
{
    public interface ICacheWrapper
    {
        Task LoadLookupsIntoCacheAsync();
        Task<List<ExpectedOutcome>> GetExpectedOutcomesFromCacheAsync();
        Task<List<CollaborationRequirement>> GetCollaborationRequirementsFromCacheAsync();
    }
}
