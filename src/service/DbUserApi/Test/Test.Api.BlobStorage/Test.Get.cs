using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Moq;
using Xunit;

namespace GarageGroup.Internal.Dataverse.Claims.Service.DbUserApi.Test;

partial class BlobStorageUserApiTest
{
    [Fact]
    public static async Task GetUserAsync_ExpectHttpApiSendCalledOnce()
    {
        var mockHttpApi = BuildMockHttpApi(SomeHttpOutput);

        var date = new DateTime(2024, 7, 3, 14, 41, 12);
        var mockDateProvider = BuildDateProvider(date);

        var option = new BlobStorageUserApiOption(
            accountName: "AccountName",
            containerName: "SomeContainerName",
            accountKey: "c29tZSBhY2NvdW50IGtleQ==");

        var api = new BlobStorageUserApi(mockHttpApi.Object, option, mockDateProvider);

        var input = new DbUserGetIn(
            azureUserId: new("829b3e83-4691-4cec-aef4-3e4641b1579c"));

        var cancellationToken = new CancellationToken(false);
        _ = await api.GetUserAsync(input, cancellationToken);

        var expectedInput = new HttpSendIn(
            method: HttpVerb.Head,
            requestUri: "https://AccountName.blob.core.windows.net/SomeContainerName/829b3e83-4691-4cec-aef4-3e4641b1579c.txt")
        {
            Headers =
            [
                new("authorization", "SharedKey AccountName:m+32YJEaaCighOY73cCc5MNfFAFAPrZzapmcZYnvYO8="),
                new("x-ms-date", "Wed, 03 Jul 2024 14:41:12 GMT"),
                new("x-ms-version", "2022-11-02")
            ]
        };

        mockHttpApi.Verify(x => x.SendAsync(expectedInput, cancellationToken), Times.Once);
    }

    [Theory]
    [MemberData(nameof(DbUserApiSource.OutputGetFailureTestData), MemberType = typeof(DbUserApiSource))]
    public static async Task GetUserAsync_HttpApiSendResultIsFailure_ExpectedFailure(
        HttpSendFailure httpSendFailure, Failure<DbUserGetFailureCode> expected)
    {
        var mockHttpApi = BuildMockHttpApi(httpSendFailure);
        var mockDateProvider = BuildDateProvider(SomeDate);

        var api = new BlobStorageUserApi(mockHttpApi.Object, SomeOption, mockDateProvider);
        var cancellationToken = new CancellationToken(false);

        var actual = await api.GetUserAsync(SomeGetInput, cancellationToken);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public static async Task GetUserAsync_HttpApiSendResultIsSuccess_ExpectedSuccess()
    {
        var httpOut = new HttpSendOut
        {
            StatusCode = HttpSuccessCode.OK,
            Headers =
            [
                new("x-ms-meta-dataverseuserid", "187f8bce-301f-416a-b35e-3fe106fb1224"),
                new("x-ms-meta-azureuserid", "36e87773-d91c-48ce-8d34-aae36312d853")
            ]
        };

        var mockHttpApi = BuildMockHttpApi(httpOut);
        var mockDateProvider = BuildDateProvider(SomeDate);

        var api = new BlobStorageUserApi(mockHttpApi.Object, SomeOption, mockDateProvider);
        var cancellationToken = new CancellationToken(false);

        var actual = await api.GetUserAsync(SomeGetInput, cancellationToken);

        var expected = new DbUserGetOut(
            dataverseUserId: new("187f8bce-301f-416a-b35e-3fe106fb1224"));

        Assert.StrictEqual(expected, actual);
    }
}