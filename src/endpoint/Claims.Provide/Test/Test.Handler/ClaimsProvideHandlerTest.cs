using Moq;
using System;
using System.Threading;

namespace GarageGroup.Internal.Dataverse.Claims.Endpoint.Claims.Provide.Test;

public static partial class ClaimsProvideHandlerTest
{
    private static readonly ClaimsProvideIn SomeInput
        =
        new()
        {
            Data = new()
            {
                AuthenticationContext = new()
                {
                    CorrelationId = Guid.Parse("1282002b-6a8b-418f-b481-67844abb0cc5"),
                    User = new()
                    {
                        Id = Guid.Parse("5b228f06-d220-4006-844a-374df853108d")
                    }
                }
            }
        };

    private static readonly DbUserGetOut SomeDbUserOutput
        =
        new(
            dataverseUserId: new("4934c84c-f0cf-447c-bee9-2bc0112fb968"));

    private static Mock<IDbUserGetSupplier> BuildDbUserApi(
        in Result<DbUserGetOut, Failure<DbUserGetFailureCode>> result)
    {
        var mock = new Mock<IDbUserGetSupplier>();

        _ = mock
            .Setup(static a => a.GetUserAsync(It.IsAny<DbUserGetIn>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return mock;
    }
}