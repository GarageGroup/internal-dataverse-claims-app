using GarageGroup.Infra;
using PrimeFuncPack;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class Application
{
    [HttpFunction("ProvideClaims", HttpMethodName.Post, Route = "provide-claims", AuthLevel = HttpAuthorizationLevel.Function)]
    internal static Dependency<IClaimsProvideHandler> UseClaimsProvideHandler()
        =>
        UseDbUserApi().UseClaimsProvideHandler();
}
