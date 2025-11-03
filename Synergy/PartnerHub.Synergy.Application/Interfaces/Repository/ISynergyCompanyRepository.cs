using System.Linq.Expressions;

namespace PartnersHub.Synergy.Application.Interfaces.Repository
{
    public interface ISynergyCompanyRepository
    {
        Task<Domain.Aggregates.SynergyCompanyAggregate.SynergyCompany?> GetByIdAsync(Guid id, bool asNoTracking = false, params Expression<Func<Domain.Aggregates.SynergyCompanyAggregate.SynergyCompany, object>>[] includes);
        Task<IEnumerable<Domain.Aggregates.SynergyCompanyAggregate.SynergyCompany>> GetAllAsync(bool asNoTracking = false);
        Task<List<Domain.Aggregates.SynergyCompanyAggregate.SynergyCompany>> GetByIdsAsync(List<Guid> ids, bool asNoTracking = false);
        
        // Dashboard methods
        Task<int> GetTotalCountAsync();
        Task<List<Domain.Aggregates.SynergyCompanyAggregate.SynergyCompany>> GetRecentAsync(int count, bool asNoTracking = true);
    }
}
