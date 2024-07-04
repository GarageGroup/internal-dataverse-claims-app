using System;

namespace GarageGroup.Internal.Dataverse.Claims;

public sealed record class ClaimsProvideOption
{
    public ClaimsProvideOption(string accountName, string accountKey)
    {
        AccountName = accountName.OrEmpty();
        AccountKey = accountKey.OrEmpty();
    }

    public string AccountName { get; }

    public string AccountKey { get; }
}
