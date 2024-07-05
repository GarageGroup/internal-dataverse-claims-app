namespace GarageGroup.Internal.Dataverse.Claims;

internal sealed partial class UsersSyncHandler : IUsersSyncHandler
{
    private readonly ICrmUserSetGetSupplier crmUserApi;

    private readonly IDbUserApi dbUserApi;

    internal UsersSyncHandler(ICrmUserSetGetSupplier crmUserApi, IDbUserApi dbUserApi)
    {
        this.crmUserApi = crmUserApi;
        this.dbUserApi = dbUserApi;
    }
}