using GarageGroup.Infra;

namespace GarageGroup.Internal.Dataverse.Claims;

internal sealed partial class CrmUserApi : ICrmUserApi
{
    private readonly ISqlQueryEntitySetSupplier sqlApi;

    private static readonly DbSelectQuery DbRequest = DbUser.QueryAll with
    {
        Filter = DbUser.DefaultFilter
    };

    internal CrmUserApi(ISqlQueryEntitySetSupplier sqlApi)
        =>
        this.sqlApi = sqlApi;
}