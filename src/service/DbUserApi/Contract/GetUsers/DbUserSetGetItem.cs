using System;

namespace GarageGroup.Internal.Dataverse.Claims;

public sealed record class DbUserSetGetItem
{
    public DbUserSetGetItem(Guid azureUserId, Guid dataverseUserId)
    {
        AzureUserId = azureUserId;
        DataverseUserId = dataverseUserId;
    }

    public Guid AzureUserId { get; }

    public Guid DataverseUserId { get; }
}