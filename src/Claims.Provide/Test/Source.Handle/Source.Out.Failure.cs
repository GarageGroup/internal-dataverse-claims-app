using System;
using System.Net.Mime;
using GarageGroup.Infra;
using Xunit;

namespace GarageGroup.Internal.Dataverse.Claims.Provide.Test;

partial class ClaimsProvideHandlerTestSource
{
    public static TheoryData<HttpSendFailure, Failure<HandlerFailureCode>> OutputFailureTestData
        =>
        new()
        {
            {
                default,
                new(
                    failureCode: HandlerFailureCode.Transient, 
                    failureMessage: "An unexpected http failure occured: 0.")
            },
            {
                new()
                {
                    StatusCode = HttpFailureCode.NotFound,
                    Body = new()
                    {
                        Type = new(MediaTypeNames.Application.Json),
                        Content = BinaryData.FromString("Some failure message")
                    }
                },
                new(
                    failureCode: HandlerFailureCode.Persistent,
                    failureMessage: "An unexpected http failure occured: 404.\nSome failure message")
            },
            {
                new()
                {
                    StatusCode = HttpFailureCode.BadRequest,
                    Body = new()
                    {
                        Type = new(MediaTypeNames.Application.Json),
                        Content = BinaryData.FromString("Some failure message")
                    }
                },
                new(
                    failureCode: HandlerFailureCode.Transient,
                    failureMessage: "An unexpected http failure occured: 400.\nSome failure message")
            },
            {
                new()
                {
                    StatusCode = HttpFailureCode.InternalServerError,
                    ReasonPhrase = "Some reason",
                    Headers =
                    [
                        new("SomeHeader", "Some value")
                    ],
                    Body = new()
                    {
                        Content = BinaryData.FromString("Some error text.")
                    }
                },
                new(
                    failureCode: HandlerFailureCode.Transient,
                    failureMessage: "An unexpected http failure occured: 500 Some reason.\nSome error text.")
            },
            {
                new()
                {
                    StatusCode = HttpFailureCode.TooManyRequests,
                    ReasonPhrase = "Some reason",
                    Headers =
                    [
                        new("SomeHeader", "Some value")
                    ],
                    Body = new()
                    {
                        Content = BinaryData.FromString("Some error text.")
                    }
                },
                new(
                    failureCode: HandlerFailureCode.Transient,
                    failureMessage: "An unexpected http failure occured: 429 Some reason.\nSome error text.")
            },
        };
}