using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class CrmUserApi
{
    public ValueTask<Result<CrmUserSetGetOut, Failure<Unit>>> GetUsersAsync(
        Unit input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            DbRequest, cancellationToken)
        .PipeValue(
            sqlApi.QueryEntitySetOrFailureAsync<DbUser>)
        .MapSuccess(
            static @out => new CrmUserSetGetOut()
            {
                Users = @out.Map(MapCrmUserSetGetItem)
            });

    private static CrmUserSetGetItem MapCrmUserSetGetItem(DbUser db)
        =>
        new(
            azureUserId: db.AzureUserId,
            dataverseUserId: db.DataverseUserId);
}