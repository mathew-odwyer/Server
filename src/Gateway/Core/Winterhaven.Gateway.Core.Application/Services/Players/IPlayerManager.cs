namespace Winterhaven.Gateway.Core.Application.Services.Players;

using System;
using System.Collections.Generic;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Players;

public interface IPlayerManager
{
    void Add(Player player);

    IReadOnlyCollection<Player> GetAllPlayers();

    Player? GetByPlayerId(Guid playerId);

    void Remove(Player player);
}