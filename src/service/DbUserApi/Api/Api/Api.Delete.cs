using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class CosmosDbUserApi
{
    public ValueTask<Result<Unit, Failure<Unit>>> DeleteUserAsync(
        DbUserDeleteIn input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}