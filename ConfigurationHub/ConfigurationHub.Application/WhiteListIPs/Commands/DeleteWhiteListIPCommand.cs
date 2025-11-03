using System;
using MediatR;
using PartnersHub.ConfigurationHub.Domain.Common;

namespace PartnersHub.ConfigurationHub.Application.WhiteListIPs.Commands {
    public record DeleteWhiteListIPCommand : IRequest<Result<bool>> {
        public Guid Id { get; init; }
    }
}