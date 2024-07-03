using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class ClaimsProvideHandler
{
    public ValueTask<Result<ClaimsProvideOut, Failure<HandlerFailureCode>>> HandleAsync(
        ClaimsProvideIn? input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private static Result<Guid, Failure<HandlerFailureCode>> ValidateUserId(ClaimsProvideIn? input)
    {
        if (input?.Data?.AuthenticationContext?.User is null)
        {
            return Failure.Create(HandlerFailureCode.Persistent, "The Azure Active Directory user is not specified");
        }

        return input.Data.AuthenticationContext.User.Id;
    }
}