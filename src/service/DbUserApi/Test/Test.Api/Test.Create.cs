using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Moq;
using Xunit;

namespace GarageGroup.Internal.Dataverse.Claims.Service.DbUserApi.Test;

partial class CosmosDbUserApiTest
{
    [Fact]
    public static async Task CreateUserAsync_ExpectHttpApiSendCalledOnce()
    {
        var mockHttpApi = BuildMockHttpApi(SomeHttpOutput);

        var date = new DateTime(2024, 7, 3, 14, 41, 12);
        var mockDateProvider = BuildDateProvider(date);

        var option = new CosmosDbUserApiOption(
            accountName: "AccountName",
            accountKey: "c29tZSBhY2NvdW50IGtleQ==");

        var api = new CosmosDbUserApi(mockHttpApi.Object, option, mockDateProvider);

        var input = new DbUserCreateIn(
            azureUserId: new("b76e756f-7f6e-4df0-b470-8f0c0a04d18c"),
            dataverseUserId: new("d0bb0ec9-6fe4-41c6-afc7-78892a24fbce"));

        var cancellationToken = new CancellationToken(false);
        _ = await api.CreateUserAsync(input, cancellationToken);

        var expectedInput = new HttpSendIn(
            method: HttpVerb.Post,
            requestUri: "https://AccountName.table.cosmos.azure.com/DataverseUsers")
        {
            Headers =
            [
                new("authorization", "SharedKeyLite AccountName:88gZ/q3ms6VWsIaZvpgzYIXrt3lg9D7rfcGsThQoul4="),
                new("x-ms-date", "Wed, 03 Jul 2024 14:41:12 GMT"),
                new("x-ms-version", "2019-02-02"),
                new("accept", "application/json;odata=nometadata")
            ],
            Body = HttpBody.SerializeAsJson(new InnerDbUserJson
            {
                DataverseUserId = new("d0bb0ec9-6fe4-41c6-afc7-78892a24fbce"),
                RowKey = new("b76e756f-7f6e-4df0-b470-8f0c0a04d18c"),
                PartitionKey = new("b76e756f-7f6e-4df0-b470-8f0c0a04d18c")
            }),
            SuccessType = HttpSuccessType.OnlyStatusCode
        };

        mockHttpApi.Verify(x => x.SendAsync(expectedInput, cancellationToken), Times.Once);
    }

    [Theory]
    [MemberData(nameof(CosmosDbUserApiSource.OutputFailureTestData), MemberType = typeof(CosmosDbUserApiSource))]
    public static async Task CreateUserAsync_HttpApiSendResultIsFailure_ExpectedFailure(
        HttpSendFailure httpSendFailure, Failure<Unit> expected)
    {
        var mockHttpApi = BuildMockHttpApi(httpSendFailure);
        var mockDateProvider = BuildDateProvider(SomeDate);

        var api = new CosmosDbUserApi(mockHttpApi.Object, SomeOption, mockDateProvider);
        var cancellationToken = new CancellationToken(false);

        var actual = await api.CreateUserAsync(SomeCreateInput, cancellationToken);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public static async Task CreateUserAsync_HttpApiSendResultIsSuccess_ExpectedSuccess()
    {
        var httpOut = new HttpSendOut
        {
            StatusCode = HttpSuccessCode.Created,
        };

        var mockHttpApi = BuildMockHttpApi(httpOut);
        var mockDateProvider = BuildDateProvider(SomeDate);

        var api = new CosmosDbUserApi(mockHttpApi.Object, SomeOption, mockDateProvider);
        var cancellationToken = new CancellationToken(false);

        var actual = await api.CreateUserAsync(SomeCreateInput, cancellationToken);

        Assert.StrictEqual(Result.Success<Unit>(default), actual);
    }
}