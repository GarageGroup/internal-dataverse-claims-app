using System;

namespace GarageGroup.Internal.Dataverse.Claims;

public sealed record class CrmUserSetGetItem
{
    public CrmUserSetGetItem(Guid azureUserId, Guid dataverseUserId)
    {
        AzureUserId = azureUserId;
        DataverseUserId = dataverseUserId;
    }

    public Guid AzureUserId { get; }

    public Guid DataverseUserId { get; }
}