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
    public static async Task CreateUserAsync_ExpectHttpApiSendCalledOnce()
    {
        var mockHttpApi = BuildMockHttpApi(SomeHttpOutput);

        var date = new DateTime(2024, 7, 3, 14, 41, 12);
        var mockDateProvider = BuildDateProvider(date);

        var option = new BlobStorageUserApiOption(
            accountName: "AccountName",
            containerName: "SomeContainerName",
            accountKey: "c29tZSBhY2NvdW50IGtleQ==");

        var api = new BlobStorageUserApi(mockHttpApi.Object, option, mockDateProvider);

        var input = new DbUserCreateIn(
            azureUserId: new("b76e756f-7f6e-4df0-b470-8f0c0a04d18c"),
            dataverseUserId: new("d0bb0ec9-6fe4-41c6-afc7-78892a24fbce"));

        var cancellationToken = new CancellationToken(false);
        _ = await api.CreateUserAsync(input, cancellationToken);

        var expectedInput = new HttpSendIn(
            method: HttpVerb.Put,
            requestUri: "https://AccountName.blob.core.windows.net/SomeContainerName/b76e756f-7f6e-4df0-b470-8f0c0a04d18c.txt")
        {
            Headers =
            [
                new("authorization", "SharedKey AccountName:HfZXg/VDhNyqPILgV5GNXcuI7OIy1TEEgcDG8tQI6iQ="),
                new("x-ms-date", "Wed, 03 Jul 2024 14:41:12 GMT"),
                new("x-ms-version", "2022-11-02"),
                new("x-ms-blob-type", "BlockBlob"),
                new("x-ms-meta-azureuserid", "b76e756f-7f6e-4df0-b470-8f0c0a04d18c"),
                new("x-ms-meta-dataverseuserid", "d0bb0ec9-6fe4-41c6-afc7-78892a24fbce")
            ],
            Body = new HttpBody()
            {
                Content = new("1"),
                Type = new("text/plain")
            },
            SuccessType = HttpSuccessType.OnlyStatusCode
        };

        mockHttpApi.Verify(x => x.SendAsync(expectedInput, cancellationToken), Times.Once);
    }

    [Theory]
    [MemberData(nameof(DbUserApiSource.OutputFailureTestData), MemberType = typeof(DbUserApiSource))]
    public static async Task CreateUserAsync_HttpApiSendResultIsFailure_ExpectedFailure(
        HttpSendFailure httpSendFailure, Failure<Unit> expected)
    {
        var mockHttpApi = BuildMockHttpApi(httpSendFailure);
        var mockDateProvider = BuildDateProvider(SomeDate);

        var api = new BlobStorageUserApi(mockHttpApi.Object, SomeOption, mockDateProvider);
        var actual = await api.CreateUserAsync(SomeCreateInput, default);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public static async Task CreateUserAsync_HttpApiSendResultIsSuccess_ExpectedSuccess()
    {
        var httpOut = new HttpSendOut
        {
            StatusCode = HttpSuccessCode.Created
        };

        var mockHttpApi = BuildMockHttpApi(httpOut);
        var mockDateProvider = BuildDateProvider(SomeDate);

        var api = new BlobStorageUserApi(mockHttpApi.Object, SomeOption, mockDateProvider);
        var actual = await api.CreateUserAsync(SomeCreateInput, default);

        Assert.StrictEqual(Result.Success<Unit>(default), actual);
    }
}
