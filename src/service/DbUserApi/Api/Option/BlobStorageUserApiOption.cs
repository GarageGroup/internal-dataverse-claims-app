using System;

namespace GarageGroup.Internal.Dataverse.Claims;

public sealed record class BlobStorageUserApiOption
{
    public BlobStorageUserApiOption(string accountName, string containerName, string accountKey)
    {
        AccountName = accountName.OrEmpty();
        ContainerName = containerName.OrEmpty();
        AccountKey = accountKey.OrEmpty();
    }

    public string AccountName { get; }

    public string ContainerName { get; }

    public string AccountKey { get; }
}