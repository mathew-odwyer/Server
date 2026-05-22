function user_logged_out(user)
{
	/// @type {Struct.Logger}
	/// @description The logger.
	static _logger = new Logger(nameof(user_logged_out));
	
	var username = user[$ "username"];	
	var player = player_get_by_name(username);
	
	if (player == noone)
	{
		return;
	}

	_logger.log(log_type.information, $"Player leaving room with name: '{username}'");
	
	instance_destroy(player);
	
	_logger.log(log_type.information, $"Player left room with name: '{username}'");
}