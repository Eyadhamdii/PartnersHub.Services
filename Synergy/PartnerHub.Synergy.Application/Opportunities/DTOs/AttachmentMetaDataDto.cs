namespace PartnersHub.Synergy.Application.Opportunities.DTOs;

public record AttachmentMetaDataDto(string fileName, string sharePointUrl, long fileSizeInBytes, string? uploadedby);
