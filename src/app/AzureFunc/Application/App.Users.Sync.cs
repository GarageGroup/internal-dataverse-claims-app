using System;
using PrimeFuncPack;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class Application
{
    internal static Dependency<IUsersSyncHandler> UseUsersSyncHandler()
        =>
        Pipeline.Pipe(
            UseCrmUserApi())
        .With(
            ResolveDbUserApi)
        .UseUsersSyncHandler();
}