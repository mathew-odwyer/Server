/// @descriptions Gets the player associated with the specified `connection`.
/// @param {Struct.ClientConnection} connection The client connection associated with the player.
/// @returns {Id.Instance.obj_player} Returns the player instance associated with the specified `connection`.
function player_get_by_connection(connection)
{
	with (obj_player)
	{
		if (self.connection == connection)
		{
			/// @feather ignore once GM1045
			return id;
		}
	}
	
	/// @feather ignore once GM1045
	return noone;
}
