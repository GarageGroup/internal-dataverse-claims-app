using System;
using System.Text.Json.Serialization;
using System.Threading;
using GarageGroup.Infra;
using Moq;

namespace GarageGroup.Internal.Dataverse.Claims.Service.DbUserApi.Test;

public static partial class CosmosDbUserApiTest
{
    private static readonly DateTime SomeDate
        =
        new(2024, 7, 3, 12, 40, 11);

    private static readonly HttpSendOut SomeHttpOutput
        =
        new()
        {
            StatusCode = HttpSuccessCode.OK,
            Body = HttpBody.SerializeAsJson(
                value: new InnerDbUserJson
                {
                    DataverseUserId = new("d0bb0ec9-6fe4-41c6-afc7-78892a24fbce"),
                    RowKey = new("b76e756f-7f6e-4df0-b470-8f0c0a04d18c"),
                    PartitionKey = new("b76e756f-7f6e-4df0-b470-8f0c0a04d18c")
                })
        };

    private static readonly CosmosDbUserApiOption SomeOption
        =
        new(
            accountName: "SomeAccountName",
            accountKey: "c29tZSBhY2NvdW50IGtleQ==");

    private static readonly DbUserGetIn SomeGetInput
        =
        new(
            azureUserId: new("5b228f06-d220-4006-844a-374df853108d"));

    private static readonly DbUserDeleteIn SomeDeleteInput
        =
        new(
            azureUserId: new("5e787817-004d-464a-a957-c2fc318b5455"));

    private static readonly DbUserCreateIn SomeCreateInput
        =
        new(
            azureUserId: new("b76e756f-7f6e-4df0-b470-8f0c0a04d18c"),
            dataverseUserId: new("d0bb0ec9-6fe4-41c6-afc7-78892a24fbce"));

    private static Mock<IHttpApi> BuildMockHttpApi(
        in Result<HttpSendOut, HttpSendFailure> result)
    {
        var mock = new Mock<IHttpApi>();

        _ = mock
            .Setup(static a => a.SendAsync(It.IsAny<HttpSendIn>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return mock;
    }

    private static IDateProvider BuildDateProvider(DateTime date)
        =>
        Mock.Of<IDateProvider>(t => t.Date == date);

    internal sealed record class InnerDataverseUserId
    {
        public Guid DataverseUserId { get; init; }
    }

    internal sealed record class InnerDbUserJson
    {
        [JsonPropertyName("DataverseUserId")]
        public Guid DataverseUserId { get; init; }

        [JsonPropertyName("RowKey")]
        public Guid RowKey { get; init; }

        [JsonPropertyName("PartitionKey")]
        public Guid PartitionKey { get; init; }
    }

    private readonly record struct InnerDbUserSetJson
    {
        [JsonPropertyName("value")]
        public FlatArray<InnerDbUserJson> Value { get; init; }
    }
}