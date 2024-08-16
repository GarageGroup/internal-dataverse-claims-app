using System;

namespace GarageGroup.Internal.Dataverse.Claims;

internal sealed class DateProvider : IDateProvider
{
    public static readonly DateProvider Instance;

    static DateProvider()
        =>
        Instance = new();

    private DateProvider()
    {
    }

    public DateTime Date
        =>
        DateTime.UtcNow;
}