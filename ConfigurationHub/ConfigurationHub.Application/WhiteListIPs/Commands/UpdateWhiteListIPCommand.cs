using System;
using MediatR;
using PartnersHub.ConfigurationHub.Domain.Common;

namespace PartnersHub.ConfigurationHub.Application.WhiteListIPs.Commands {
    public record UpdateWhiteListIPCommand : IRequest<Result<bool>> {
        public Guid Id { get; init; }
        public string? Description { get; init; }
        public DateTime? ExpiryDate { get; init; }
        public Guid UpdatedBy { get; init; }
    }
}