using GarageGroup.Infra;

namespace GarageGroup.Internal.Dataverse.Claims;

[DbEntity("systemuser", AliasName)]
internal sealed partial record class DbUser : IDbEntity<DbUser>
{
    private const string All = "QueryAll";

    private const string AliasName = "u";
}
