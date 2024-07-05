using GarageGroup.Infra;

namespace GarageGroup.Internal.Dataverse.Claims;

internal sealed partial class CrmUserApi : ICrmUserApi
{
    private readonly ISqlQueryEntitySetSupplier sqlApi;

    internal CrmUserApi(ISqlQueryEntitySetSupplier sqlApi)
        =>
        this.sqlApi = sqlApi;
}