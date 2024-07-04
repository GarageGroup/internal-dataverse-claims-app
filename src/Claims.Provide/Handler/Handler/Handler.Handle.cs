using System;
using System.Security.Cryptography;
using System.Text;
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
            BuildHttpSendIn)
        .ForwardValue(
            httpApi.SendAsync,
            static failure => failure.ToStandardFailure().MapFailureCode(MapFailureCodeWhenSearchingForUser))
        .MapSuccess(
            systemUser => new ClaimsProvideOut(
                data: new()
                {
                    Actions =
                    [
                        new(
                            claims: new(
                                correlationId: input?.Data?.AuthenticationContext?.CorrelationId ?? default,
                                systemUserId: systemUser.Body.DeserializeFromJson<SystemUserId>().Id))
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

    private HttpSendIn BuildHttpSendIn(Guid userId)
    {
        var date = dateProvider.Date.ToString("R");
        var stringToSign = $"{date}\n/{option.AccountName}/{TableName}(PartitionKey='{userId}',RowKey='{userId}')";

        using var hashAlgorithm = new HMACSHA256(Convert.FromBase64String(option.AccountKey));
        var hash = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
        var signature = Convert.ToBase64String(hash);

        return new(
            method: HttpVerb.Get,
            requestUri: $"https://{option.AccountName}.table.cosmos.azure.com/" +
                $"{TableName}(PartitionKey='{userId}',RowKey='{userId}')?$select={SelectedField}")
        {
            Headers =
            [
                new("Authorization", $"SharedKeyLite {option.AccountName}:{signature}"),
                new("x-ms-date", date),
                new("x-ms-version", ApiVersion),
                new("Accept", AcceptHeader)
            ]
        };
    }

    private static HandlerFailureCode MapFailureCodeWhenSearchingForUser(HttpFailureCode failureCode)
        =>
        failureCode switch
        {
            HttpFailureCode.NotFound => HandlerFailureCode.Persistent,
            _ => HandlerFailureCode.Transient
        };
}