using System;
using GarageGroup.Infra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrimeFuncPack;

namespace GarageGroup.Internal.Dataverse.Claims;

internal static partial class Application
{
    private static Dependency<ICrmUserApi> UseCrmUserApi()
        =>
        UseDataverseSqlApi().UseCrmUserApi();

    //private static Dependency<IDbUserApi> UseDbUserApi()
    //    =>
    //    PrimaryHandler.UseStandardSocketsHttpHandler()
    //    .UseLogging("CosmosDbHttpApi")
    //    .UsePollyStandard()
    //    .UseHttpApi()
    //    .With(ResolveCosmosDbUserApiOption)
    //    .UseCosmosDbUserApi();

    private static Dependency<IDbUserApi> UseDbUserApi()
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging("BlobStorageHttpApi", HttpLoggerType.RequestHeaders)
        .UsePollyStandard()
        .UseHttpApi()
        .With(ResolveBlobStorageUserApiOption)
        .UseBlobStorageUserApi();

    private static Dependency<ISqlApi> UseDataverseSqlApi()
        =>
        DataverseDbProvider.Configure("Dataverse").UseSqlApi();

    private static CosmosDbUserApiOption ResolveCosmosDbUserApiOption(IServiceProvider serviceProvider)
    {
        var section = serviceProvider.GetRequiredService<IConfiguration>().GetRequiredSection("CosmosDb");
        
        return new(
            accountName: section["AccountName"].OrEmpty(),
            accountKey: section["AccountKey"].OrEmpty());
    }

    private static BlobStorageUserApiOption ResolveBlobStorageUserApiOption(IServiceProvider serviceProvider)
    {
        var section = serviceProvider.GetRequiredService<IConfiguration>().GetRequiredSection("BlobStorage");

        return new(
            accountName: section["AccountName"].OrEmpty(),
            containerName: section["ContainerName"].OrEmpty(),
            accountKey: section["AccountKey"].OrEmpty());
    }
}