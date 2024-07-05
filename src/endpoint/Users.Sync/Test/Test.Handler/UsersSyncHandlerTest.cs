using System;
using System.Threading;
using Moq;

namespace GarageGroup.Internal.Dataverse.Claims.Endpoint.Users.Sync.Test;

public static partial class UsersSyncHandlerTest
{
    private static readonly CrmUserSetGetOut SomeCrmUserSet
        =
        new()
        {
            Users =
            [
                new(
                    azureUserId: new("0a75315d-e184-4c9d-8e4e-7b4eb7be7958"),
                    dataverseUserId: new("cf3f38ac-abcb-4e17-bbad-9e3d9dd00109")),
                new(
                    azureUserId: new("e4c18a17-cfb3-47b9-8437-232448997557"),
                    dataverseUserId: new("6f8bd3c4-fce4-4e44-bc48-1218df891b76"))
            ]
        };

    private static readonly DbUserSetGetOut SomeDbUserSet
        =
        new()
        {
            Users =
            [
                new(
                    azureUserId: new("e4c18a17-cfb3-47b9-8437-232448997557"),
                    dataverseUserId: new("6f8bd3c4-fce4-4e44-bc48-1218df891b76")),
                new(
                    azureUserId: new("03b7a217-43d4-48f8-8465-30f426f11acf"),
                    dataverseUserId: new("2b5b5b38-0244-4c1f-8ec9-956b8c35e764"))
            ]
        };

    private static Mock<ICrmUserSetGetSupplier> BuildMockCrmUserApi(
        in Result<CrmUserSetGetOut, Failure<Unit>> result)
    {
        var mock = new Mock<ICrmUserSetGetSupplier>();

        _ = mock
            .Setup(static a => a.GetUsersAsync(default, It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return mock;
    }

    private static Mock<IDbUserApi> BuildMockDbUserApi(
        in Result<DbUserSetGetOut, Failure<Unit>> dbUserSetGetResult,
        in Result<Unit, Failure<Unit>> dbUserCreateResult,
        in Result<Unit, Failure<Unit>> dbUserDeleteResult)
    {
        var mock = new Mock<IDbUserApi>();

        _ = mock
            .Setup(static a => a.GetUsersAsync(default, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbUserSetGetResult);

        _ = mock
            .Setup(static a => a.CreateUserAsync(It.IsAny<DbUserCreateIn>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbUserCreateResult);

        _ = mock
            .Setup(static a => a.DeleteUserAsync(It.IsAny<DbUserDeleteIn>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbUserDeleteResult);

        return mock;
    }
}