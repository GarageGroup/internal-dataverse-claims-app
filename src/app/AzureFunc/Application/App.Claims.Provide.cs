using PrimeFuncPack;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class Application
{
    internal static Dependency<IClaimsProvideHandler> UseClaimsProvideHandler()
        =>
        Dependency.From(
            ResolveDbUserApi)
        .UseClaimsProvideHandler();
}