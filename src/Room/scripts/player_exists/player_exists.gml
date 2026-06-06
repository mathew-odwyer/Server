/// @description Determines whether a player that matches the specified `identifier` exists.
/// @param {String} identifier The unique identifier of the player.
/// @returns {Bool} Returns `true` if the player exists; otherwise, `false`.
function player_exists(identifier)
{
    return player_get_by_identifier(identifier) != noone;
}
