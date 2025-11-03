using Microsoft.EntityFrameworkCore;
using PartnersHub.ConfigurationHub.Application.Common.Interfaces.Repositories;
using PartnersHub.ConfigurationHub.Domain.Aggregates.Configuration;

namespace PartnersHub.ConfigurationHub.Infrastructure.Persistence.Repositories;

public class WhiteListIPRepository : IWhiteListIPRepository {
    private readonly ConfigurationHubDbContext _context;

    public WhiteListIPRepository(ConfigurationHubDbContext context) {
        _context = context;
    }

    public async Task<WhiteListIP?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        return await _context.WhiteListIPs
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<WhiteListIP>> GetAllAsync(CancellationToken cancellationToken = default) {
        return await _context.WhiteListIPs
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WhiteListIP>> GetActiveAsync(CancellationToken cancellationToken = default) {
        return await _context.WhiteListIPs
            .Where(w => w.IsActive && w.ExpiryDate > DateTime.UtcNow)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<WhiteListIP?> GetByIPAddressAsync(string ipAddress, CancellationToken cancellationToken = default) {
        // Query the owned IPAddress value object
        return await _context.WhiteListIPs
            .Where(w => EF.Property<string>(w.IPAddress, "Value") == ipAddress)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> IsIPWhitelistedAsync(string ipAddress, CancellationToken cancellationToken = default) {
        return await _context.WhiteListIPs
            .AnyAsync(w => EF.Property<string>(w.IPAddress, "Value") == ipAddress
                          && w.IsActive
                          && w.ExpiryDate > DateTime.UtcNow,
                cancellationToken);
    }

    public async Task AddAsync(WhiteListIP whiteListIP, CancellationToken cancellationToken = default) {
        await _context.WhiteListIPs.AddAsync(whiteListIP, cancellationToken);
    }

    public void Update(WhiteListIP whiteListIP) {
        _context.WhiteListIPs.Update(whiteListIP);
    }

    public void Delete(WhiteListIP whiteListIP) {
        _context.WhiteListIPs.Remove(whiteListIP);
    }
}