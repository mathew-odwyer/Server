/// @description Logs out the user associated with the request.
/// @param {Undefined} _ Unusued parameters.
/// @param {Struct.ClientConnection} connection The client connection that sent the request.
/// @returns {Struct.__Promise} Returns a promise that resolves when the player logs out.
function user_logout(_, connection)
{
	/// @type {Struct.Logger}
	/// @description The logger.
	static _logger = new Logger(nameof(user_logout));	
	
	var client_options = {
		bearer: connection[$ "access_token"],
		jsonrpc_error: true,
	};

	if (!is_undefined(connection[$ "access_timer"]))
	{
		call_cancel(connection[$ "access_timer"]);
	}

	var user_account_client = new UserAccountClient(client_options);
	var player_client = new PlayerClient(client_options);

	var player = player_get(connection);

	var dto = {
		x: player[$ "x"] ?? undefined,
		y: player[$ "y"] ?? undefined,
	};

	return player_client.update_async(dto)
		.next(user_account_client.logout_async)
		.next(method({player, _logger}, function()
		{
			instance_destroy(player);
		}));
}
