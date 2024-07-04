using System;

namespace GarageGroup.Internal.Dataverse.Claims;

internal interface IDateProvider
{
    DateTime Date { get; }
}