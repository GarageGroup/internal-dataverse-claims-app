using GarageGroup.Infra;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GarageGroup.Internal.Dataverse.Claims.Provide.Test;

partial class ClaimsProvideHandlerTest
{
    [Fact]
    public static async Task HandleAsync_InputIsInvalid_ExpectValidationFailure()
    {
        var mockHttpApi = BuildMockHttpApi(SomeHttpOutput);
        var mockDateProvider = BuildDateProvider(SomeDate);

        var handler = new ClaimsProvideHandler(mockHttpApi.Object, SomeOption, mockDateProvider);
        var cancellationToken = new CancellationToken(false);

        var input = new ClaimsProvideIn
        {
            Data = new()
        };

        var actual = await handler.HandleAsync(input, cancellationToken);
        var expected = Failure.Create(HandlerFailureCode.Persistent, "The Azure Active Directory user is not specified");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public static async Task HandleAsync_InputIsValid_ExpectHttpApiSendCalledOnce()
    {
        var httpOut = new HttpSendOut()
        {
            StatusCode = HttpSuccessCode.OK,
            Body = HttpBody.SerializeAsJson(
                value: new InnerSystemUserId
                {
                    DataverseUserId = new("5b75dbfc-8a4b-40bf-babc-503dd533ae04")
                })
        };

        var mockHttpApi = BuildMockHttpApi(httpOut);

        var date = new DateTime(2024, 7, 3, 14, 41, 12);
        var mockDateProvider = BuildDateProvider(date);

        var option = new ClaimsProvideOption(
            accountName: "AccountName",
            accountKey: "c29tZSBhY2NvdW50IGtleQ==");

        var handler = new ClaimsProvideHandler(mockHttpApi.Object, option, mockDateProvider);
        
        var cancellationToken = new CancellationToken(false);
        var input = new ClaimsProvideIn
        {
            Data = new()
            {
                AuthenticationContext = new()
                {
                    CorrelationId = Guid.Parse("fd0b6a08-b75e-4378-9bf7-6c14f7aa4f27"),
                    User = new()
                    {
                        Id = Guid.Parse("b76e756f-7f6e-4df0-b470-8f0c0a04d18c")
                    }
                }
            }
        };

        _ = await handler.HandleAsync(input, cancellationToken);

        var expectedInput = new HttpSendIn(
            method: HttpVerb.Get,
            requestUri: "https://AccountName.table.cosmos.azure.com/DataverseUsers" +
                "(PartitionKey='b76e756f-7f6e-4df0-b470-8f0c0a04d18c',RowKey='b76e756f-7f6e-4df0-b470-8f0c0a04d18c')?$select=DataverseUserId")
        {
            Headers =
            [
                new("authorization", "SharedKeyLite AccountName:uOIQZAiwwjm0b4GsDZBV0kxDGbliMMNIz8pJFoXS5K4="),
                new("x-ms-date", "Wed, 03 Jul 2024 14:41:12 GMT"),
                new("x-ms-version", "2019-02-02"),
                new("accept", "application/json;odata=nometadata")
            ]
        };

        mockHttpApi.Verify(x => x.SendAsync(expectedInput, cancellationToken), Times.Once);
    }

    [Theory]
    [MemberData(nameof(ClaimsProvideHandlerTestSource.OutputFailureTestData), MemberType = typeof(ClaimsProvideHandlerTestSource))]
    public static async Task HandleAsync_HttpApiSendResultIsFailure_ExpectedFailure(
        HttpSendFailure httpSendFailure, Failure<HandlerFailureCode> expected)
    {
        var mockHttpApi = BuildMockHttpApi(httpSendFailure);
        var mockDateProvider = BuildDateProvider(SomeDate);

        var handler = new ClaimsProvideHandler(mockHttpApi.Object, SomeOption, mockDateProvider);
        var cancellationToken = new CancellationToken(false);

        var actual = await handler.HandleAsync(SomeInput, cancellationToken);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public static async Task HandleAsync_HttpApiSendResultIsSuccess_ExpectedSuccess()
    {
        var httpOut = new HttpSendOut
        {
            StatusCode = HttpSuccessCode.OK,
            Body = HttpBody.SerializeAsJson(
                value: new InnerSystemUserId
                {
                    DataverseUserId = new("5b75dbfc-8a4b-40bf-babc-503dd533ae04")
                }),
        };

        var mockHttpApi = BuildMockHttpApi(httpOut);
        var mockDateProvider = BuildDateProvider(SomeDate);

        var handler = new ClaimsProvideHandler(mockHttpApi.Object, SomeOption, mockDateProvider);
        var cancellationToken = new CancellationToken(false);

        var input = new ClaimsProvideIn
        {
            Data = new()
            {
                AuthenticationContext = new()
                {
                    CorrelationId = Guid.Parse("fd0b6a08-b75e-4378-9bf7-6c14f7aa4f27"),
                    User = new()
                    {
                        Id = Guid.Parse("b76e756f-7f6e-4df0-b470-8f0c0a04d18c")
                    }
                }
            }
        };

        var actual = await handler.HandleAsync(input, cancellationToken);

        var expected = new ClaimsProvideOut(
            data: new()
            {
                Actions =
                [
                    new(
                        claims: new(
                            correlationId: new("fd0b6a08-b75e-4378-9bf7-6c14f7aa4f27"),
                            systemUserId: new("5b75dbfc-8a4b-40bf-babc-503dd533ae04")))
                ]
            });

        Assert.StrictEqual(expected, actual);
    }
}