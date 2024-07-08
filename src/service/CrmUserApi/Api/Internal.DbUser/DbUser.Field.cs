using GarageGroup.Infra;
using System;

namespace GarageGroup.Internal.Dataverse.Claims;

partial record class DbUser
{
    [DbSelect(All, AliasName, $"{AliasName}.systemuserid")]
    public Guid DataverseUserId { get; init; }

    [DbSelect(All, AliasName, $"{AliasName}.azureactivedirectoryobjectid")]
    public Guid AzureUserId { get; init; }
}