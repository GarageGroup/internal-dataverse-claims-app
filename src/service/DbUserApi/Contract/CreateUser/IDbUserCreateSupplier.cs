using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Dataverse.Claims;

public interface IDbUserCreateSupplier
{
    ValueTask<Result<Unit, Failure<Unit>>> CreateUserAsync(
        DbUserCreateIn input, CancellationToken cancellationToken);
}