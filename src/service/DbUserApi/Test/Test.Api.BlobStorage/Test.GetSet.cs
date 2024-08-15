using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Moq;
using Xunit;

namespace GarageGroup.Internal.Dataverse.Claims.Service.DbUserApi.Test;

partial class BlobStorageUserApiTest
{
    [Theory]
    [MemberData(nameof(DbUserApiSource.GetSetInputTestData), MemberType = typeof(DbUserApiSource))]
    public static async Task GetUsersAsync_ExpectHttpApiSendCalledOnce(
        BlobStorageUserApiOption option, 
        DateTime date, 
        FlatArray<HttpSendIn> expectedInputs, 
        FlatArray<Result<HttpSendOut, HttpSendFailure>> httpOutputs)
    {
        var mockHttpApi = BuildMockHttpApi(httpOutputs);
        var mockDateProvider = BuildDateProvider(date);

        var api = new BlobStorageUserApi(mockHttpApi.Object, option, mockDateProvider);

        var cancellationToken = new CancellationToken(false);
        _ = await api.GetUsersAsync(default, cancellationToken);

        foreach (var expectedInput in expectedInputs)
        {
            mockHttpApi.Verify(x => x.SendAsync(expectedInput, cancellationToken), Times.Once);
        }        
    }

    [Theory]
    [MemberData(nameof(DbUserApiSource.OutputFailureTestData), MemberType = typeof(DbUserApiSource))]
    public static async Task GetUsersAsync_HttpApiSendResultIsFailure_ExpectedFailure(
        HttpSendFailure httpSendFailure, Failure<Unit> expected)
    {
        var mockHttpApi = BuildMockHttpApi(httpSendFailure);
        var mockDateProvider = BuildDateProvider(SomeDate);

        var api = new BlobStorageUserApi(mockHttpApi.Object, SomeOption, mockDateProvider);
        var cancellationToken = new CancellationToken(false);

        var actual = await api.GetUsersAsync(default, cancellationToken);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(DbUserApiSource.GetSetOutTestData), MemberType = typeof(DbUserApiSource))]
    public static async Task GetUsersAsync_HttpApiSendResultIsSuccess_ExpectedSuccess(
        FlatArray<Result<HttpSendOut, HttpSendFailure>> httpOutputs,
        DbUserSetGetOut expected)
    {
        var mockHttpApi = BuildMockHttpApi(httpOutputs);
        var mockDateProvider = BuildDateProvider(SomeDate);

        var api = new BlobStorageUserApi(mockHttpApi.Object, SomeOption, mockDateProvider);
        var actual = await api.GetUsersAsync(default, default);

        Assert.StrictEqual(expected, actual);
    }
}
