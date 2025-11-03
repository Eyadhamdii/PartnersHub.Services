using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PartnersHub.Synergy.Application.Interfaces.Common;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Domain.Aggregates.Synergy.Lookups;
using PartnersHub.Synergy.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CacheWrapper: ICacheWrapper
{
    private readonly IMemoryCache _cache;
    private readonly ICollaborationRequirementRepository _collaborationRequirementRepository;
    private readonly IExpectedOutcomesRepository _expectedOutcomesRepository;
    private CancellationTokenSource _resetCacheToken = new();

    public CacheWrapper(
        IMemoryCache cache,
        SynergyDbContext context,
        ICollaborationRequirementRepository collaborationRequirementRepository,
        IExpectedOutcomesRepository expectedOutcomesRepository)
    {
        _cache = cache;
        _collaborationRequirementRepository = collaborationRequirementRepository;
        _expectedOutcomesRepository = expectedOutcomesRepository;
    }

    public async Task LoadLookupsIntoCacheAsync()
    {
        // Load CollaborationRequirements into cache
        var collaborationRequirements = await _collaborationRequirementRepository.GetAllAsync();
        var collaborationRequirementsCacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(2));
        _cache.Set("CollaborationRequirements", collaborationRequirements, collaborationRequirementsCacheEntryOptions);

        // Load ExpectedOutcomes into cache
        var expectedOutcomes = await _expectedOutcomesRepository.GetAllAsync();
        var expectedOutcomesCacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(2));
        _cache.Set("ExpectedOutcomes", expectedOutcomes, expectedOutcomesCacheEntryOptions);
    }

    public async Task<List<CollaborationRequirement>> GetCollaborationRequirementsFromCacheAsync()
    {
        if (_cache.TryGetValue("CollaborationRequirements", out List<CollaborationRequirement> collaborationRequirements))
        {
            return collaborationRequirements;
        }
        else
        {
            collaborationRequirements = await _collaborationRequirementRepository.GetAllAsync();
            var collaborationRequirementsCacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(2));
            _cache.Set("CollaborationRequirements", collaborationRequirements, collaborationRequirementsCacheEntryOptions);
            return collaborationRequirements;
        }
    }

    public async Task<List<ExpectedOutcome>> GetExpectedOutcomesFromCacheAsync()
    {
        if (_cache.TryGetValue("ExpectedOutcomes", out List<ExpectedOutcome> expectedOutcomes))
        {
            return expectedOutcomes;
        }
        else
        {
            expectedOutcomes = await _expectedOutcomesRepository.GetAllAsync();
            var expectedOutcomesCacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(2));
            _cache.Set("ExpectedOutcomes", expectedOutcomes, expectedOutcomesCacheEntryOptions);
            return expectedOutcomes;
        }
    }
    public void Clear()
    {
        _resetCacheToken.Cancel(); // this triggers the CancellationChangeToken to expire every item from cache

        _resetCacheToken.Dispose(); // dispose the current cancellation token source and create a new one
        _resetCacheToken = new CancellationTokenSource();
    }
}
