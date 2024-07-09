using GarageGroup.Infra;
using Moq;
using System;
using System.Threading;

namespace GarageGroup.Internal.Dataverse.Claims.Service.CrmUserApi.Test;

public static partial class CrmUserApiTest
{
    private static readonly FlatArray<DbUser> SomeUserSetOutput
        =
        [
            new()
            {
                DataverseUserId = new("8a7f359c-5c2f-4285-8b0a-5438f7e3934e"),
                AzureUserId = new("84cf4bf5-bc56-4a59-8434-20bfbffd8ab0")
            },
            new()
            {
                DataverseUserId = new("6d870098-3243-468a-892b-499fc903fec4"),
                AzureUserId = new("70243163-5a9f-4db7-8342-ec1579281036")
            }
        ];

    private static Mock<ISqlQueryEntitySetSupplier> BuildMockSqlApi(
        in Result<FlatArray<DbUser>, Failure<Unit>> result)
    {
        var mock = new Mock<ISqlQueryEntitySetSupplier>();

        _ = mock
            .Setup(static a => a.QueryEntitySetOrFailureAsync<DbUser>(It.IsAny<IDbQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return mock;
    }
}