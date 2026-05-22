/// @description Determines whether a player that matches the specified `name` exists.
/// @param {String} name The name of the player.
/// @returns {Bool} Returns `true` if the player exists; otherwise, `false`.
function player_exists(name)
{
	if (string_empty(name))
	{
		return false;
	}
	
	return player_get_by_name(name) != noone;
}