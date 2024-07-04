using System;
using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Dataverse.Claims.Provide.Test;

internal readonly record struct StubSystemUserId
{
    [JsonPropertyName("DataverseUserId")]
    public Guid Id { get; init; }
}