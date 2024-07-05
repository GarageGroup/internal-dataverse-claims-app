using GarageGroup.Infra;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GarageGroup.Internal.Dataverse.Claims.Endpoint.Claims.Provide.Test;

partial class ClaimsProvideHandlerTest
{
    [Theory]
    [MemberData(nameof(ClaimsProvideHandlerSource.InputInvalidTestData), MemberType = typeof(ClaimsProvideHandlerSource))]
    public static async Task HandleAsync_InputIsInvalid_ExpectPersistentFailure(
        ClaimsProvideIn? input)
    {
        var mockDbUserApi = BuildDbUserApi(SomeDbUserOutput);
        var handler = new ClaimsProvideHandler(mockDbUserApi.Object);

        var cancellationToken = new CancellationToken(false);

        var actual = await handler.HandleAsync(input, cancellationToken);
        var expected = Failure.Create(HandlerFailureCode.Persistent, "The Azure Active Directory user is not specified");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public static async Task HandleAsync_InputIsValid_ExpectDbUserGetCalledOnce()
    {
        var mockDbUserApi = BuildDbUserApi(SomeDbUserOutput);
        var handler = new ClaimsProvideHandler(mockDbUserApi.Object);

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

        var cancellationToken = new CancellationToken(false);
        _ = await handler.HandleAsync(input, cancellationToken);

        var expectedInput = new DbUserGetIn(
            azureUserId: new("b76e756f-7f6e-4df0-b470-8f0c0a04d18c"));

        mockDbUserApi.Verify(a => a.GetUserAsync(expectedInput, cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData(DbUserGetFailureCode.NotFound, HandlerFailureCode.Persistent)]
    [InlineData(DbUserGetFailureCode.Unknown, HandlerFailureCode.Transient)]
    public static async Task HandleAsync_DbUserGetResultIsFailure_ExpectedFailure(
        DbUserGetFailureCode dbUserGetFailureCode, HandlerFailureCode expectedFailureCode)
    {
        var sourceException = new Exception("Some Error");
        var dbUserGetFailure = sourceException.ToFailure(dbUserGetFailureCode, "Some message");

        var mockDbUserApi = BuildDbUserApi(dbUserGetFailure);
        var handler = new ClaimsProvideHandler(mockDbUserApi.Object);

        var actual = await handler.HandleAsync(SomeInput, default);
        var expected = Failure.Create(expectedFailureCode, "Some message", sourceException);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public static async Task HandleAsync_DbUserGetResultIsSuccess_ExpectedSuccess()
    {
        var dbUser = new DbUserGetOut(
            dataverseUserId: new("5b75dbfc-8a4b-40bf-babc-503dd533ae04"));

        var mockDbUserApi = BuildDbUserApi(dbUser);
        var handler = new ClaimsProvideHandler(mockDbUserApi.Object);

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

        var actual = await handler.HandleAsync(input, default);

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