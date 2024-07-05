using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Dataverse.Claims;

public interface IDbUserSetGetSupplier
{
    ValueTask<Result<DbUserSetGetOut, Failure<Unit>>> GetUsersAsync(
        Unit input, CancellationToken cancellationToken);
}