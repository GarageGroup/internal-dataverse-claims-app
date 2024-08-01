using GarageGroup.Infra;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class Function
{
    [Function("ProvideClaims")]
    public static Task<HttpResponseData> ProvideClaimsAsync(
        [HttpTrigger(AuthorizationLevel.Function, "POST", Route = "provide-claims")] HttpRequestData requestData,
        CancellationToken cancellationToken)
        =>
        Application.UseClaimsProvideHandler()
        .RunHttpFunctionAsync<IClaimsProvideHandler, ClaimsProvideIn, ClaimsProvideOut>(requestData, cancellationToken);
}