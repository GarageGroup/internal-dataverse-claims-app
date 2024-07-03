using GarageGroup.Infra;
using PrimeFuncPack;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class Application
{
    [HttpFunction("HealthCheck", HttpMethodName.Get, Route = "health", AuthLevel = HttpAuthorizationLevel.Function)]
    internal static Dependency<IHealthCheckHandler> UseHealthCheckHandler()
        =>
        HealthCheck.UseEmpty().UseHealthCheckHandler();
}