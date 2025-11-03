namespace PartnersHub.Synergy.Application.Models;

public record KeyValueDto<TKey>(TKey Id, string Name);
public record KeyValueDto(int Id, string Name);
public record GuidKeyValueDto(Guid Id, string Name);
