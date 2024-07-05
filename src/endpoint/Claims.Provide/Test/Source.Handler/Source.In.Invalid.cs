using Xunit;

namespace GarageGroup.Internal.Dataverse.Claims.Endpoint.Claims.Provide.Test;

partial class ClaimsProvideHandlerSource
{
    public static TheoryData<ClaimsProvideIn?> InputInvalidTestData
        =>
        new()
        {
            {
                null
            },
            {
                new()
                {
                    Data = null
                }
            },
            {
                new()
                {
                    Data = new()
                    {
                        AuthenticationContext = null
                    }
                }
            },
            {
                new()
                {
                    Data = new()
                    {
                        AuthenticationContext = new()
                        {
                            User = null
                        }
                    }
                }
            }
        };
}