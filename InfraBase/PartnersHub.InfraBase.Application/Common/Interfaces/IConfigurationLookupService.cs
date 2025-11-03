using System;
using System.Threading;
using System.Threading.Tasks;

namespace PartnersHub.InfraBase.Application.Common.Interfaces;

public interface IConfigurationLookupService
{
    Task<string?> GetSectorNameAsync(Guid sectorId, CancellationToken cancellationToken = default);
    Task<string?> GetSubSectorNameAsync(Guid subSectorId, CancellationToken cancellationToken = default);
    Task<string?> GetAssetTypeNameAsync(Guid assetTypeId, CancellationToken cancellationToken = default);
    Task<string?> GetUomNameAsync(Guid uomId, CancellationToken cancellationToken = default);
}