using GarageGroup.Infra;
using Moq;
using System;
using System.Threading;

namespace GarageGroup.Internal.Dataverse.Claims.Provide.Test;

public static partial class ClaimsProvideHandlerTest
{
    private static readonly DateTime SomeDate
        =
        new(2024, 7, 3, 12, 40, 11);

    private static readonly HttpSendOut SomeHttpOutput
        =
        new()
        {
            StatusCode = HttpSuccessCode.OK,
            Body = HttpBody.SerializeAsJson(new StubSystemUserId()
            {
                Id = new("4934c84c-f0cf-447c-bee9-2bc0112fb968")
            }),
        };

    private static readonly ClaimsProvideOption SomeOption
        =
        new(
            accountName: "SomeAccountName",
            accountKey: "c29tZSBhY2NvdW50IGtleQ==",
            tableName: "SomeTableName");

    private static readonly ClaimsProvideIn SomeInput
        =
        new()
        {
            Data = new()
            {
                AuthenticationContext = new()
                {
                    CorrelationId = Guid.Parse("1282002b-6a8b-418f-b481-67844abb0cc5"),
                    User = new()
                    {
                        Id = Guid.Parse("5b228f06-d220-4006-844a-374df853108d")
                    }
                }
            }
        };

    private static Mock<IHttpApi> BuildMockHttpApi(
        in Result<HttpSendOut, HttpSendFailure> result)
    {
        var mock = new Mock<IHttpApi>();

        _ = mock.Setup(static a => a.SendAsync(It.IsAny<HttpSendIn>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        return mock;
    }

    private static IDateProvider BuildDateProvider(DateTime date)
        =>
        Mock.Of<IDateProvider>(t => t.Date == date);
}