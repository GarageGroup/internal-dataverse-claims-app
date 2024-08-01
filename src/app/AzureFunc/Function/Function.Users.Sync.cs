using GarageGroup.Infra;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class Function
{
    [Function("SyncUsersByHttp")]
    public static Task<HttpResponseData> SyncUsersByHttpAsync(
        [HttpTrigger(AuthorizationLevel.Function, "POST", Route = "sync-users")] HttpRequestData requestData,
        CancellationToken cancellationToken)
        =>
        Application.UseUsersSyncHandler()
        .RunHttpFunctionAsync<IUsersSyncHandler, Unit, Unit>(requestData, cancellationToken);
}