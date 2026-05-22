/// @description Gets a player by the specified `name`.
/// @param {String} name The name of the player.
/// @returns {Id.Instance.obj_player} Returns an `obj_player` that matches the specified `name`; otherwise, `undefined`.
function player_get_by_name(name)
{
	if (string_empty(name))
	{
		return noone;
	}
	
	with (obj_player)
	{
		// Ignore case-sensitivity to ensure we don't have false matches or miss a player.
		if (string_upper(self.name) == string_upper(name))
		{
			return id;
		}
	}
	
	return noone;
}