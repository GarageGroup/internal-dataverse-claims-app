using System;

namespace GarageGroup.Internal.Dataverse.Claims;

public sealed record class DbUserDeleteIn
{
    public DbUserDeleteIn(Guid azureUserId)
        =>
        AzureUserId = azureUserId;

    public Guid AzureUserId { get; }
}