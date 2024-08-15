using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using GarageGroup.Infra;
using Moq;

namespace GarageGroup.Internal.Dataverse.Claims.Service.DbUserApi.Test;

public static partial class BlobStorageUserApiTest
{
    private static readonly DateTime SomeDate
        =
        new(2024, 8, 15, 13, 20, 21);

    private static readonly HttpSendOut SomeHttpOutput
        =
        new()
        {
            StatusCode = HttpSuccessCode.OK,
            Body = new HttpBody()
            {
                Content = new(""),
                Type = new(MediaTypeNames.Text.Xml)
            },
            Headers =
            [
                new("x-ms-meta-dataverseuserid", "187f8bce-301f-416a-b35e-3fe106fb1224"),
                new("x-ms-meta-azureuserid", "36e87773-d91c-48ce-8d34-aae36312d853")
            ]
        };

    private static readonly BlobStorageUserApiOption SomeOption
        =
        new(
            accountName: "SomeAccountName",
            containerName: "SomeContainerName",
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

    private static Mock<IHttpApi> BuildMockHttpApi(
        in FlatArray<Result<HttpSendOut, HttpSendFailure>> results)
    {
        var queue = new Queue<Result<HttpSendOut, HttpSendFailure>>(results.AsEnumerable());

        var mock = new Mock<IHttpApi>();
        _ = mock.Setup(static a => a.SendAsync(It.IsAny<HttpSendIn>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queue.Dequeue);

        return mock;
    }

    private static IDateProvider BuildDateProvider(DateTime date)
        =>
        Mock.Of<IDateProvider>(t => t.Date == date);
}