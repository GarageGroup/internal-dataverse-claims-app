using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Dataverse.Claims;

public interface ICrmUserSetGetSupplier
{
    ValueTask<Result<CrmUserSetGetOut, Failure<Unit>>> GetUsersAsync(
        Unit input, CancellationToken cancellationToken);
}