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
    public static async Task GetUsersAsync_ExpectHttpApiSendCalledOnce()
    {
        var mockHttpApi = BuildMockHttpApi(SomeHttpOutput);

        var date = new DateTime(2024, 7, 3, 14, 41, 12);
        var mockDateProvider = BuildDateProvider(date);

        var option = new CosmosDbUserApiOption(
            accountName: "AccountName",
            accountKey: "c29tZSBhY2NvdW50IGtleQ==");

        var api = new CosmosDbUserApi(mockHttpApi.Object, option, mockDateProvider);

        var cancellationToken = new CancellationToken(false);
        _ = await api.GetUsersAsync(default, cancellationToken);

        var expectedInput = new HttpSendIn(
            method: HttpVerb.Get,
            requestUri: "https://AccountName.table.cosmos.azure.com/DataverseUsers")
        {
            Headers =
            [
                new("authorization", "SharedKeyLite AccountName:88gZ/q3ms6VWsIaZvpgzYIXrt3lg9D7rfcGsThQoul4="),
                new("x-ms-date", "Wed, 03 Jul 2024 14:41:12 GMT"),
                new("x-ms-version", "2019-02-02"),
                new("accept", "application/json;odata=nometadata")
            ]
        };

        mockHttpApi.Verify(x => x.SendAsync(expectedInput, cancellationToken), Times.Once);
    }

    [Theory]
    [MemberData(nameof(DbUserApiSource.OutputFailureTestData), MemberType = typeof(DbUserApiSource))]
    public static async Task GetUsersAsync_HttpApiSendResultIsFailure_ExpectedFailure(
        HttpSendFailure httpSendFailure, Failure<Unit> expected)
    {
        var mockHttpApi = BuildMockHttpApi(httpSendFailure);
        var mockDateProvider = BuildDateProvider(SomeDate);

        var api = new CosmosDbUserApi(mockHttpApi.Object, SomeOption, mockDateProvider);
        var cancellationToken = new CancellationToken(false);

        var actual = await api.GetUsersAsync(default, cancellationToken);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public static async Task GetUsersAsync_HttpApiSendResultIsSuccess_ExpectedSuccess()
    {
        var httpOut = new HttpSendOut
        {
            StatusCode = HttpSuccessCode.OK,
            Body = HttpBody.SerializeAsJson(
                value: new InnerDbUserSetJson
                {
                    Value =
                    [
                        new()
                        {
                            DataverseUserId = new("496175b7-4a73-47c5-a892-72866dbeed7f"),
                            RowKey = new("04363ce1-3d1b-4fbd-b5ad-e4a67c271445"),
                            PartitionKey = new("04363ce1-3d1b-4fbd-b5ad-e4a67c271445")
                        },
                        new()
                        {
                            DataverseUserId = new("2a8feb24-c7b8-4046-88fe-2fa0d2992b58"),
                            RowKey = new("0cdc1b29-7c58-43d9-9fde-f39bdbd1fd06"),
                            PartitionKey = new("0cdc1b29-7c58-43d9-9fde-f39bdbd1fd06")
                        }
                    ]
                })
        };

        var mockHttpApi = BuildMockHttpApi(httpOut);
        var mockDateProvider = BuildDateProvider(SomeDate);

        var api = new CosmosDbUserApi(mockHttpApi.Object, SomeOption, mockDateProvider);

        var actual = await api.GetUsersAsync(default, default);

        var expected = new DbUserSetGetOut
        {
            Users =
            [
                new(
                    azureUserId: new("04363ce1-3d1b-4fbd-b5ad-e4a67c271445"),
                    dataverseUserId: new("496175b7-4a73-47c5-a892-72866dbeed7f")),
                new(
                    azureUserId: new("0cdc1b29-7c58-43d9-9fde-f39bdbd1fd06"),
                    dataverseUserId: new("2a8feb24-c7b8-4046-88fe-2fa0d2992b58"))
            ]
        };

        Assert.StrictEqual(expected, actual);
    }
}
