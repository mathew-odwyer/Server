namespace Winterhaven.Gateway.Core.Domain.ValueObjects.Players;

using System;

public sealed record Player(
    Guid connectionId,
    Guid playerId,
    string Name,
    double X,
    double Y);