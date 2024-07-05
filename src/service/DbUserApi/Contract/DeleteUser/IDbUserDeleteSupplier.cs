using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Dataverse.Claims;

public interface IDbUserDeleteSupplier
{
    ValueTask<Result<Unit, Failure<Unit>>> DeleteUserAsync(
        DbUserDeleteIn input, CancellationToken cancellationToken);
}