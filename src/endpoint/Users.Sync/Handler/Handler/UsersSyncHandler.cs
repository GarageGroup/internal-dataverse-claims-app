using System;
using System.Collections.Generic;
using System.Linq;

namespace GarageGroup.Internal.Dataverse.Claims;

internal sealed partial class UsersSyncHandler : IUsersSyncHandler
{
    private static readonly PipelineParallelOption ParallelOption
        =
        new()
        {
            DegreeOfParallelism = 4
        };

    private readonly ICrmUserSetGetSupplier crmUserApi;

    private readonly IDbUserApi dbUserApi;

    internal UsersSyncHandler(ICrmUserSetGetSupplier crmUserApi, IDbUserApi dbUserApi)
    {
        this.crmUserApi = crmUserApi;
        this.dbUserApi = dbUserApi;
    }

    private static IEnumerable<DbUserCreateIn> GetUsersToCreate(CrmUserSetGetOut crm, DbUserSetGetOut db)
    {
        var dbUserIds = db.Users.AsEnumerable().Select(GetAzureUserId).ToList();
        var usersToCreate = new List<DbUserCreateIn>(crm.Users.Length);

        foreach (var user in crm.Users)
        {
            if (dbUserIds.Contains(user.AzureUserId))
            {
                continue;
            }

            yield return new(
                azureUserId: user.AzureUserId,
                dataverseUserId: user.DataverseUserId);
        }

        static Guid GetAzureUserId(DbUserSetGetItem user)
            =>
            user.AzureUserId;
    }

    private static IEnumerable<DbUserDeleteIn> GetUsersToDelete(CrmUserSetGetOut crm, DbUserSetGetOut db)
    {
        var crmUserIds = crm.Users.AsEnumerable().Select(GetAzureUserId).ToList();
        var usersToDelete = new List<DbUserDeleteIn>(db.Users.Length);

        foreach (var user in db.Users)
        {
            if (crmUserIds.Contains(user.AzureUserId))
            {
                continue;
            }

            yield return new(
                azureUserId: user.AzureUserId);
        }

        static Guid GetAzureUserId(CrmUserSetGetItem user)
            =>
            user.AzureUserId;
    }

    private sealed record class UserOperationSet
    {
        public required FlatArray<DbUserCreateIn> UsersToCreate { get; init; }

        public required FlatArray<DbUserDeleteIn> UsersToDelete { get; init; }
    }
}