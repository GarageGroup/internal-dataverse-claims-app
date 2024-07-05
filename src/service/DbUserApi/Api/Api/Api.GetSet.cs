using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class CosmosDbUserApi
{
    public ValueTask<Result<DbUserSetGetOut, Failure<Unit>>> GetUsersAsync(
        Unit input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}