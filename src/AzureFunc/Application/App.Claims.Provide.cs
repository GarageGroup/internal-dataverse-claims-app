using System;
using GarageGroup.Infra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrimeFuncPack;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class Application
{
    [HttpFunction("ProvideClaims", HttpMethodName.Post, Route = "provide-claims", AuthLevel = HttpAuthorizationLevel.Function)]
    internal static Dependency<IClaimsProvideHandler> UseClaimsProvideHandler()
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging("CosmosDbHttpApi")
        .UsePollyStandard()
        .UseHttpApi()
        .With(ResolveClaimsProvideOption)
        .UseClaimsProvideHandler();

    private static ClaimsProvideOption ResolveClaimsProvideOption(IServiceProvider serviceProvider)
    {
        var section = serviceProvider.GetRequiredService<IConfiguration>().GetRequiredSection("CosmosDb");
        
        return new(
            accountName: section["AccountName"].OrEmpty(),
            accountKey: section["AccountKey"].OrEmpty(),
            tableName: section["TableName"].OrEmpty());
    }
}
