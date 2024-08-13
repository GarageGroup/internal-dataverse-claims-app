using GarageGroup.Infra;

namespace GarageGroup.Internal.Dataverse.Claims;

partial record class DbUser
{
    internal readonly static DbRawFilter DefaultFilter
        =
        new(
            $"{AliasName}.applicationid IS NULL " +
            $"AND {AliasName}.isdisabled = 0 " +
            $"AND {AliasName}.azureactivedirectoryobjectid IS NOT NULL");
}