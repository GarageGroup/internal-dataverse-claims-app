using GarageGroup.Infra;
using PrimeFuncPack;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class Application
{
    internal static Dependency<IHealthCheckHandler> UseHealthCheckHandler()
        =>
        UseDataverseSqlApi().UseServiceHealthCheckApi("DataverseSqlApi").UseHealthCheckHandler();
}