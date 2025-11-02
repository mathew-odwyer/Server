/// @description Refreshes an existing users JWT.
/// @param {String} refresh_token The refresh token.
/// @param {Struct.ClientConnection} connection The client connection that sent the request.
/// @returns {Struct.__Promise} Returns a promise that is resolved when the API responds.
function user_refresh(refresh_token, connection)
{
	/// @type {Struct.Logger}
	/// @description The logger.
	static _logger = new Logger(nameof(user_refresh));
	
	var user_account_client = new UserAccountClient({
		bearer: connection[$ "access_token"],
		jsonrpc_error: true,
	});
	
	return user_account_client.refresh_token_async({ refresh_token: refresh_token })
		.next(method({connection, _logger}, function(result)
		{
			connection[$ "access_token"] = result[$ "access_token"];
			connection[$ "refresh_token"] = result[$ "refresh_token"];
			
			// Ensure that the current access timer is cancelled.
			if (!is_undefined(connection[$ "access_timer"]))
			{
				call_cancel(connection[$ "access_timer"]);
			}
			
			connection[$ "access_timer"] = call_later(result[$ "expiration_seconds"] ?? 0, time_source_units_seconds, connection.cleanup, false);
			
			return connection[$ "refresh_token"];
		}));
}
