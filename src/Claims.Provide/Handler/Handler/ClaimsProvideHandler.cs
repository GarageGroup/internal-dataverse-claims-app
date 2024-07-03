using GarageGroup.Infra;

namespace GarageGroup.Internal.Dataverse.Claims;

internal sealed partial class ClaimsProvideHandler : IClaimsProvideHandler
{
    private readonly IHttpApi httpApi;

    private readonly ClaimsProvideOption option;

    internal ClaimsProvideHandler(IHttpApi httpApi, ClaimsProvideOption option)
    {
        this.httpApi = httpApi;
        this.option = option;
    }
}