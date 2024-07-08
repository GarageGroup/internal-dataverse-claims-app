using GarageGroup.Infra;

namespace GarageGroup.Internal.Dataverse.Claims;

partial record class DbUser
{
    internal static DbCombinedFilter BuildDefaultFilter()
        =>
        new(DbLogicalOperator.And)
        {
            Filters =
            [
                new DbRawFilter($"{AliasName}.title is not null"),
                new DbParameterFilter($"{AliasName}.isdisabled", DbFilterOperator.Equal, 0, "isDisabled"),
                new DbRawFilter($"{AliasName}.azureactivedirectoryobjectid is not null"),
            ]
        };
}