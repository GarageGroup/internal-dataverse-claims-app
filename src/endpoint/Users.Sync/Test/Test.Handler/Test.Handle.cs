using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Moq;
using Xunit;

namespace GarageGroup.Internal.Dataverse.Claims.Endpoint.Users.Sync.Test;

partial class UsersSyncHandlerTest
{
    [Fact]
    public static async Task HandleAsync_ExpectCrmUserSetGetCalledOnce()
    {
        var mockCrmUserApi = BuildMockCrmUserApi(SomeCrmUserSet);
        var mockDbUserApi = BuildMockDbUserApi(SomeDbUserSet, default(Unit), default(Unit));

        var handler = new UsersSyncHandler(mockCrmUserApi.Object, mockDbUserApi.Object);
        _ = await handler.HandleAsync(default, default);

        mockCrmUserApi.Verify(static a => a.GetUsersAsync(default, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public static async Task HandleAsync_CrmUserSetGetResultIsFailure_ExpectTransientFailure()
    {
        var sourceException = new Exception("Some error text");
        var sourceFailure = sourceException.ToFailure("Some Failure message");

        var mockCrmUserApi = BuildMockCrmUserApi(sourceFailure);
        var mockDbUserApi = BuildMockDbUserApi(SomeDbUserSet, default(Unit), default(Unit));

        var handler = new UsersSyncHandler(mockCrmUserApi.Object, mockDbUserApi.Object);

        var actual = await handler.HandleAsync(default, default);
        var expected = Failure.Create(HandlerFailureCode.Transient, "Some Failure message", sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task HandleAsync_ExpectDbUserSetGetCalledOnce()
    {
        var mockCrmUserApi = BuildMockCrmUserApi(SomeCrmUserSet);
        var mockDbUserApi = BuildMockDbUserApi(SomeDbUserSet, default(Unit), default(Unit));

        var handler = new UsersSyncHandler(mockCrmUserApi.Object, mockDbUserApi.Object);
        _ = await handler.HandleAsync(default, default);

        mockDbUserApi.Verify(static a => a.GetUsersAsync(default, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public static async Task HandleAsync_DbUserSetGetResultIsFailure_ExpectTransientFailure()
    {
        var mockCrmUserApi = BuildMockCrmUserApi(SomeCrmUserSet);

        var sourceException = new Exception("Some error message");
        var sourceFailure = sourceException.ToFailure("Some failure");

        var mockDbUserApi = BuildMockDbUserApi(sourceFailure, default(Unit), default(Unit));
        var handler = new UsersSyncHandler(mockCrmUserApi.Object, mockDbUserApi.Object);

        var actual = await handler.HandleAsync(default, default);
        var expected = Failure.Create(HandlerFailureCode.Transient, "Some failure", sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(UsersSyncHandlerSource.DbUsersToCreateTestData), MemberType = typeof(UsersSyncHandlerSource))]
    public static async Task HandleAsync_UserSetGetResultsAreSuccesses_ExpectDbUserCreateCalledExactTimes(
        CrmUserSetGetOut crmUserSet, DbUserSetGetOut dbUserSet, FlatArray<DbUserCreateIn> expectedInputs)
    {
        var mockCrmUserApi = BuildMockCrmUserApi(crmUserSet);
        var mockDbUserApi = BuildMockDbUserApi(dbUserSet, default(Unit), default(Unit));

        var handler = new UsersSyncHandler(mockCrmUserApi.Object, mockDbUserApi.Object);
        _ = await handler.HandleAsync(default, default);

        mockDbUserApi.Verify(
            static a => a.CreateUserAsync(It.IsAny<DbUserCreateIn>(), It.IsAny<CancellationToken>()),
            Times.Exactly(expectedInputs.Length));

        foreach (var expectedInput in expectedInputs)
        {
            mockDbUserApi.Verify(a => a.CreateUserAsync(expectedInput, It.IsAny<CancellationToken>()));
        }
    }

    [Fact]
    public static async Task HandleAsync_DbUserCreateResultIsFailure_ExpectTransientFailure()
    {
        var mockCrmUserApi = BuildMockCrmUserApi(SomeCrmUserSet);

        var sourceException = new Exception("Some error message");
        var sourceFailure = sourceException.ToFailure("Some failure message");

        var mockDbUserApi = BuildMockDbUserApi(SomeDbUserSet, sourceFailure, default(Unit));
        var handler = new UsersSyncHandler(mockCrmUserApi.Object, mockDbUserApi.Object);

        var actual = await handler.HandleAsync(default, default);
        var expected = Failure.Create(HandlerFailureCode.Transient, "Some failure message", sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Theory]
    [MemberData(nameof(UsersSyncHandlerSource.DbUsersToDeleteTestData), MemberType = typeof(UsersSyncHandlerSource))]
    public static async Task HandleAsync_UserSetGetResultsAreSuccesses_ExpectDbUserDeleteCalledExactTimes(
        CrmUserSetGetOut crmUserSet, DbUserSetGetOut dbUserSet, FlatArray<DbUserDeleteIn> expectedInputs)
    {
        var mockCrmUserApi = BuildMockCrmUserApi(crmUserSet);
        var mockDbUserApi = BuildMockDbUserApi(dbUserSet, default(Unit), default(Unit));

        var handler = new UsersSyncHandler(mockCrmUserApi.Object, mockDbUserApi.Object);
        _ = await handler.HandleAsync(default, default);

        mockDbUserApi.Verify(
            static a => a.DeleteUserAsync(It.IsAny<DbUserDeleteIn>(), It.IsAny<CancellationToken>()),
            Times.Exactly(expectedInputs.Length));

        foreach (var expectedInput in expectedInputs)
        {
            mockDbUserApi.Verify(a => a.DeleteUserAsync(expectedInput, It.IsAny<CancellationToken>()));
        }
    }

    [Fact]
    public static async Task HandleAsync_DbUserDeleteResultIsFailure_ExpectTransientFailure()
    {
        var mockCrmUserApi = BuildMockCrmUserApi(SomeCrmUserSet);

        var sourceException = new Exception("Some error");
        var sourceFailure = sourceException.ToFailure("Some failure text");

        var mockDbUserApi = BuildMockDbUserApi(SomeDbUserSet, default(Unit), sourceFailure);
        var handler = new UsersSyncHandler(mockCrmUserApi.Object, mockDbUserApi.Object);

        var actual = await handler.HandleAsync(default, default);
        var expected = Failure.Create(HandlerFailureCode.Transient, "Some failure text", sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task HandleAsync_AllResultsAreSuccesses_ExpectSuccess()
    {
        var mockCrmUserApi = BuildMockCrmUserApi(SomeCrmUserSet);
        var mockDbUserApi = BuildMockDbUserApi(SomeDbUserSet, default(Unit), default(Unit));

        var handler = new UsersSyncHandler(mockCrmUserApi.Object, mockDbUserApi.Object);

        var actual = await handler.HandleAsync(default, default);
        var expected = Result.Success<Unit>(default);

        Assert.StrictEqual(expected, actual);
    }
}