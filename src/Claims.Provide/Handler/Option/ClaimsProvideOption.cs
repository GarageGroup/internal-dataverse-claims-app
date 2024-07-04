using System;

namespace GarageGroup.Internal.Dataverse.Claims;

public sealed record class ClaimsProvideOption
{
    public ClaimsProvideOption(string accountName, string accountKey, string tableName)
    {
        AccountName = accountName.OrEmpty();
        AccountKey = accountKey.OrEmpty();
        TableName = tableName.OrEmpty();
    }

    public string AccountName { get; }

    public string AccountKey { get; }

    public string TableName { get; }
}
