using PrimeFuncPack;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class Application
{
    internal static Dependency<IClaimsProvideHandler> UseClaimsProvideHandler()
        =>
        UseDbUserApi().UseClaimsProvideHandler();
}