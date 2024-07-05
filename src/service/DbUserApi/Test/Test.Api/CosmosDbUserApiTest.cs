using System;
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
                value: new InnerDataverseUserId
                {
                    DataverseUserId = new("4934c84c-f0cf-447c-bee9-2bc0112fb968")
                }),
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
}