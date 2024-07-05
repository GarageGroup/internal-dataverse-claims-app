using System;
using Xunit;

namespace GarageGroup.Internal.Dataverse.Claims.Endpoint.Users.Sync.Test;

partial class UsersSyncHandlerSource
{
    public static TheoryData<CrmUserSetGetOut, DbUserSetGetOut, FlatArray<DbUserCreateIn>> DbUsersToCreateTestData
        =>
        new()
        {
            {
                default,
                default,
                default
            },
            {
                default,
                new()
                {
                    Users =
                    [
                        new(
                            azureUserId: new("75dd90aa-2f24-41be-b40d-5690e3bc81e2"),
                            dataverseUserId: new("72d606eb-1b3a-4f6a-b0ad-3f69be9724e0"))
                    ]
                },
                default
            },
            {
                new()
                {
                    Users =
                    [
                        new(
                            azureUserId: new("a0a1edfc-4823-4d46-822d-e4a2263b76e2"),
                            dataverseUserId: new("cc8339d1-fe03-44ae-8852-6c5229446302")),
                        new(
                            azureUserId: new("42938c7f-2a07-4732-a4d4-d627b1e363d7"),
                            dataverseUserId: new("b75fb725-54be-4733-8b09-b4f2f0ce6e53")),
                        new(
                            azureUserId: new("c4c6bbe2-e124-4a44-8936-f857933cc9c9"),
                            dataverseUserId: new("395875b7-3570-4cad-bc92-79c671a725b2"))
                    ]
                },
                new()
                {
                    Users =
                    [
                        new(
                            azureUserId: new("c4c6bbe2-e124-4a44-8936-f857933cc9c9"),
                            dataverseUserId: new("395875b7-3570-4cad-bc92-79c671a725b2")),
                        new(
                            azureUserId: new("a0a1edfc-4823-4d46-822d-e4a2263b76e2"),
                            dataverseUserId: new("cc8339d1-fe03-44ae-8852-6c5229446302")),
                        new(
                            azureUserId: new("42938c7f-2a07-4732-a4d4-d627b1e363d7"),
                            dataverseUserId: new("15150759-2773-4b19-a06b-b0bffa89e9f2"))
                    ]
                },
                default
            },
            {
                new()
                {
                    Users =
                    [
                        new(
                            azureUserId: new("657f3c0c-391b-4849-92ca-49f09466db86"),
                            dataverseUserId: new("3953e26c-cd4e-4e02-a9aa-209a82559b83")),
                        new(
                            azureUserId: new("c30faf2a-d203-46ec-8aa3-bf1960891b15"),
                            dataverseUserId: new("b277d44b-d260-466a-9a62-3eb933945982")),
                        new(
                            azureUserId: new("9517e163-a4f2-4d63-9961-fd638d99850c"),
                            dataverseUserId: new("25f5efda-16aa-4d38-b6d3-d4855a818692"))
                    ]
                },
                new()
                {
                    Users =
                    [
                        new(
                            azureUserId: new("c30faf2a-d203-46ec-8aa3-bf1960891b15"),
                            dataverseUserId: new("b277d44b-d260-466a-9a62-3eb933945982")),
                        new(
                            azureUserId: new("cf386414-c3cb-47f4-bb52-bffdab31100e"),
                            dataverseUserId: new("a806c2a3-d739-4cb2-8fc2-b12c020fc326"))
                    ]
                },
                [
                    new(
                        azureUserId: new("657f3c0c-391b-4849-92ca-49f09466db86"),
                        dataverseUserId: new("3953e26c-cd4e-4e02-a9aa-209a82559b83")),
                    new(
                        azureUserId: new("9517e163-a4f2-4d63-9961-fd638d99850c"),
                        dataverseUserId: new("25f5efda-16aa-4d38-b6d3-d4855a818692"))
                ]
            }
        };
}