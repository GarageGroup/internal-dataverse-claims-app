using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Microsoft.Azure.Functions.Worker;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class Function
{
    [Function("SyncUsersByTimer")]
    [FixedDelayRetry(5, "00:00:10")]
    public static Task SyncUsersByTimerAsync(
        [TimerTrigger("0 0 * * * *")] JsonElement timerInfo,
        FunctionContext context,
        CancellationToken cancellationToken)
        =>
        Application.UseUsersSyncHandler().RunAzureFunctionAsync<IUsersSyncHandler, Unit, Unit>(timerInfo, context, cancellationToken);
}