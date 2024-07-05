using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class CrmUserApi
{
    public ValueTask<Result<CrmUserSetGetOut, Failure<Unit>>> GetUsersAsync(
        Unit input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}