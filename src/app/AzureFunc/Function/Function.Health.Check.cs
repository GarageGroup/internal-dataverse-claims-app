using GarageGroup.Infra;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class Function
{
    [Function("HealthCheck")]
    public static Task<HttpResponseData> HealthCheckAsync([HttpTrigger(AuthorizationLevel.Function, "GET", Route = "health")] HttpRequestData requestData, CancellationToken cancellationToken)
        =>
        Application.UseHealthCheckHandler()
        .RunHttpFunctionAsync<IHealthCheckHandler, Unit, String>(requestData, cancellationToken);
}