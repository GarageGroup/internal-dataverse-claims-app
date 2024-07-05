namespace GarageGroup.Internal.Dataverse.Claims;

public sealed record class ClaimsProvideIn
{
    public AuthenticationEventData? Data { get; init; }
}