using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Dataverse.Claims;

public interface IDbUserGetSupplier
{
    ValueTask<Result<DbUserGetOut, Failure<DbUserGetFailureCode>>> GetUserAsync(
        DbUserGetIn input, CancellationToken cancellationToken);
}