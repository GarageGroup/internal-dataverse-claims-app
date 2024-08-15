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
    public static async Task DeleteUserAsync_ExpectHttpApiSendCalledOnce()
    {
        var mockHttpApi = BuildMockHttpApi(SomeHttpOutput);

        var date = new DateTime(2024, 7, 3, 14, 41, 12);
        var mockDateProvider = BuildDateProvider(date);

        var option = new BlobStorageUserApiOption(
            accountName: "AccountName",
            containerName: "SomeContainerName",
            accountKey: "c29tZSBhY2NvdW50IGtleQ==");

        var api = new BlobStorageUserApi(mockHttpApi.Object, option, mockDateProvider);

        var input = new DbUserDeleteIn(
            azureUserId: new("b76e756f-7f6e-4df0-b470-8f0c0a04d18c"));

        var cancellationToken = new CancellationToken(false);
        _ = await api.DeleteUserAsync(input, cancellationToken);

        var expectedInput = new HttpSendIn(
            method: HttpVerb.Delete,
            requestUri: "https://AccountName.blob.core.windows.net/SomeContainerName/b76e756f-7f6e-4df0-b470-8f0c0a04d18c.txt")
        {
            Headers =
            [
                new("authorization", "SharedKey AccountName:tXUy6wXrnOew38h8iQU65tpPOWRBFtepmccNX4bOIJs="),
                new("x-ms-date", "Wed, 03 Jul 2024 14:41:12 GMT"),
                new("x-ms-version", "2022-11-02"),
                new("x-ms-delete-snapshots", "include")
            ],
            SuccessType = HttpSuccessType.OnlyStatusCode
        };

        mockHttpApi.Verify(x => x.SendAsync(expectedInput, cancellationToken), Times.Once);
    }

    [Theory]
    [MemberData(nameof(DbUserApiSource.OutputFailureTestData), MemberType = typeof(DbUserApiSource))]
    public static async Task DeleteUserAsync_HttpApiSendResultIsFailure_ExpectedFailure(
        HttpSendFailure httpSendFailure, Failure<Unit> expected)
    {
        var mockHttpApi = BuildMockHttpApi(httpSendFailure);
        var mockDateProvider = BuildDateProvider(SomeDate);

        var api = new BlobStorageUserApi(mockHttpApi.Object, SomeOption, mockDateProvider);
        var actual = await api.DeleteUserAsync(SomeDeleteInput, default);

        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task DeleteUserAsync_HttpApiSendResultIsSuccess_ExpectedSuccess()
    {
        var httpOut = new HttpSendOut
        {
            StatusCode = HttpSuccessCode.NoContent,
        };

        var mockHttpApi = BuildMockHttpApi(httpOut);
        var mockDateProvider = BuildDateProvider(SomeDate);

        var api = new BlobStorageUserApi(mockHttpApi.Object, SomeOption, mockDateProvider);

        var actual = await api.DeleteUserAsync(SomeDeleteInput, default);
        var expected = Result.Success<Unit>(default);

        Assert.StrictEqual(expected, actual);
    }
}
