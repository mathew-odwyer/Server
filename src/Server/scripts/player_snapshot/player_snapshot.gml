/// @description Gets a player snapshot.
/// @param {Any} player The player data or instance to get a snapshot from.
/// @returns {Struct} Returns a snapshot of the specified `player`.
function player_snapshot(player)
{
    return {
        name: player.name,
        x: 64,//player.x,
        y: 64,//player.y,
    };
}
