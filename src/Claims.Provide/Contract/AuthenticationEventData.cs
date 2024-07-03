namespace GarageGroup.Internal.Dataverse.Claims;

public sealed record class AuthenticationEventData
{
    public AuthenticationContext? AuthenticationContext { get; init; }
}