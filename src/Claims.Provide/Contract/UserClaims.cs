using System;

namespace GarageGroup.Internal.Dataverse.Claims;

public sealed record class UserClaims
{
    public UserClaims(Guid correlationId, Guid systemUserId)
    {
        CorrelationId = correlationId;
        SystemUserId = systemUserId;
    }

    public Guid CorrelationId { get; }

    public Guid SystemUserId { get; }
}