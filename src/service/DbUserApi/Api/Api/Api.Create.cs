using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class CosmosDbUserApi
{
    public ValueTask<Result<Unit, Failure<Unit>>> CreateUserAsync(
        DbUserCreateIn input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}