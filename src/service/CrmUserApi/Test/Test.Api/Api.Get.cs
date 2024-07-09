using GarageGroup.Infra;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GarageGroup.Internal.Dataverse.Claims.Service.CrmUserApi.Test;

partial class CrmUserApiTest
{
    [Fact]
    public static async Task GetUsersAsync_ExpectSqlApiCalledOnce()
    {
        var mockSql = BuildMockSqlApi(SomeUserSetOutput);
        var api = new Claims.CrmUserApi(mockSql.Object);

        var cancellationToken = new CancellationToken(false);
        _ = await api.GetUsersAsync(default, cancellationToken);

        var expectedInput = new DbSelectQuery("systemuser", "u")
        {
            SelectedFields = new(
                "u.systemuserid AS DataverseUserId",
                "u.azureactivedirectoryobjectid AS AzureUserId"),
            Filter = new DbRawFilter("u.title IS NOT NULL AND u.isdisabled = 0 AND u.azureactivedirectoryobjectid IS NOT NULL")
        };
        mockSql.Verify(a => a.QueryEntitySetOrFailureAsync<DbUser>(expectedInput, cancellationToken), Times.Once);
    }

    [Fact]
    public static async Task GetUsersAsync_DbResultIsFailure_ExpectUnknownFailure()
    {
        var sourceException = new Exception("Some error message");
        var dbFailure = sourceException.ToFailure("Some Failure message");

        var mockSqlApi = BuildMockSqlApi(dbFailure);
        var api = new Claims.CrmUserApi(mockSqlApi.Object);

        var actual = await api.GetUsersAsync(default, default);
        var expected = Failure.Create(default(Unit), "Some Failure message", sourceException);

        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task GetUsersAsync_DbResultIsSuccess_ExpectSeccess()
    {
        FlatArray<DbUser> dbOut =
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

        var mockSql = BuildMockSqlApi(dbOut);
        var api = new Claims.CrmUserApi(mockSql.Object);

        var actual = await api.GetUsersAsync(default, default);
        var expected = new CrmUserSetGetOut()
        {
            Users =
            [
                new(
                    azureUserId: new("84cf4bf5-bc56-4a59-8434-20bfbffd8ab0"),
                    dataverseUserId:  new("8a7f359c-5c2f-4285-8b0a-5438f7e3934e")),
                new(
                    azureUserId: new("70243163-5a9f-4db7-8342-ec1579281036"),
                    dataverseUserId:  new("6d870098-3243-468a-892b-499fc903fec4"))
            ]
        };

        Assert.StrictEqual(expected, actual);
    }
}