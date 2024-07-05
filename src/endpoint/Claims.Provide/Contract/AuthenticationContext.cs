using System;

namespace GarageGroup.Internal.Dataverse.Claims;

public sealed record class AuthenticationContext
{
    public Guid CorrelationId { get; init; }
    
    public User? User { get; init; }
}