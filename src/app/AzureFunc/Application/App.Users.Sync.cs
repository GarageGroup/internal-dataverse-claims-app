using System;
using GarageGroup.Infra;
using PrimeFuncPack;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class Application
{
    [HttpFunction("SyncUsers", HttpMethodName.Post, Route = "sync-users", AuthLevel = HttpAuthorizationLevel.Function)]
    internal static Dependency<IUsersSyncHandler> UseUsersSyncHandler()
        =>
        Pipeline.Pipe(
            UseCrmUserApi())
        .With(
            UseDbUserApi())
        .UseUsersSyncHandler();
}