using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class UsersSyncHandler
{
    public ValueTask<Result<Unit, Failure<HandlerFailureCode>>> HandleAsync(
        Unit input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .PipeParallelValue(
            crmUserApi.GetUsersAsync,
            dbUserApi.GetUsersAsync)
        .MapSuccess(
            static @out => new UserOperationSet
            {
                UsersToCreate = GetUsersToCreate(@out.Item1, @out.Item2).ToFlatArray(),
                UsersToDelete = GetUsersToDelete(@out.Item1, @out.Item2).ToFlatArray()
            })
        .ForwardParallelValue(
            CreateUsersAsync,
            DeleteUsersAsync)
        .Map(
            Unit.From,
            static failure => failure.WithFailureCode(HandlerFailureCode.Transient));

    private ValueTask<Result<Unit, Failure<Unit>>> CreateUsersAsync(
        UserOperationSet operationSet, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            operationSet.UsersToCreate, cancellationToken)
        .PipeParallelValue(
            dbUserApi.CreateUserAsync,
            ParallelOption)
        .MapSuccess(
            Unit.From);

    private ValueTask<Result<Unit, Failure<Unit>>> DeleteUsersAsync(
        UserOperationSet operationSet, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            operationSet.UsersToDelete, cancellationToken)
        .PipeParallelValue(
            dbUserApi.DeleteUserAsync,
            ParallelOption)
        .MapSuccess(
            Unit.From);
}