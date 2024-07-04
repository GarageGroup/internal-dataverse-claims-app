using System.Diagnostics.CodeAnalysis;
using System;

namespace GarageGroup.Internal.Dataverse.Claims;

public sealed record class ClaimsProvideOption
{
    public ClaimsProvideOption(
        [AllowNull] string accountName,
        [AllowNull] string accountKey,
        [AllowNull] string tableName)
    {
        AccountName = accountName.OrEmpty();
        AccountKey = accountKey.OrEmpty();
        TableName = tableName.OrEmpty();
    }
    public string AccountName { get; }

    public string AccountKey { get; }

    public string TableName { get; }
}