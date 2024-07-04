using GarageGroup.Infra;
using System.Text.Json.Serialization;
using System;

namespace GarageGroup.Internal.Dataverse.Claims;

internal sealed partial class ClaimsProvideHandler : IClaimsProvideHandler
{
    private const string SelectedField = "DataverseUserId";

    private const string ApiVersion = "2019-02-02";

    private const string AcceptHeader = "application/json;odata=nometadata";

    private readonly IHttpApi httpApi;

    private readonly ClaimsProvideOption option;

    private readonly IDateProvider dateProvider;

    internal ClaimsProvideHandler(IHttpApi httpApi, ClaimsProvideOption option, IDateProvider dateProvider)
    {
        this.httpApi = httpApi;
        this.option = option;
        this.dateProvider = dateProvider;
    }

    private readonly record struct SystemUserId
    {
        [JsonPropertyName(SelectedField)]
        public Guid Id { get; init; }
    }
}