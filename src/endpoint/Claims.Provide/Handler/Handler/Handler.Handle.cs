using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class ClaimsProvideHandler
{
    public ValueTask<Result<ClaimsProvideOut, Failure<HandlerFailureCode>>> HandleAsync(
        ClaimsProvideIn? input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            ValidateUserId)
        .MapSuccess(
            static azureUserId => new DbUserGetIn(azureUserId))
        .ForwardValue(
            dbUserApi.GetUserAsync,
            static failure => failure.MapFailureCode(MapFailureCode))
        .MapSuccess(
            success => new ClaimsProvideOut(
                data: new()
                {
                    Actions =
                    [
                        new(
                            claims: new(
                                correlationId: input?.Data?.AuthenticationContext?.CorrelationId ?? default,
                                systemUserId: success.DataverseUserId))
                    ]
                }));

    private static Result<Guid, Failure<HandlerFailureCode>> ValidateUserId(ClaimsProvideIn? input)
    {
        if (input?.Data?.AuthenticationContext?.User is null)
        {
            return Failure.Create(HandlerFailureCode.Persistent, "The Azure Active Directory user is not specified");
        }

        return input.Data.AuthenticationContext.User.Id;
    }

    private static HandlerFailureCode MapFailureCode(DbUserGetFailureCode failureCode)
        =>
        failureCode switch
        {
            DbUserGetFailureCode.NotFound => HandlerFailureCode.Persistent,
            _ => HandlerFailureCode.Transient
        };
}