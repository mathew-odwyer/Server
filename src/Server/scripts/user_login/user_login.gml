/// @description Authenticates an existing user account.
/// @param {Struct} credentials The user account credentials.
/// @param {Struct.ClientConnection} connection The client connection that sent the request.
/// @returns {Struct.__Promise} Returns a promise that is resolved when the user logs in.
function user_login(credentials, connection)
{
	/// @type {Struct.Logger}
	/// @description The logger.
	static _logger = new Logger(nameof(user_login));

	if (!is_struct(credentials))
	{
		return;
	}

	var user_account_client = new UserAccountClient({
		jsonrpc_error: true,
	});
	
	_logger.log(log_type.information, $"Authenticating user: '{credentials[$ "username"]}'");

	return user_account_client.login_async(credentials)
		.next(method({connection, _logger}, function(result)
		{
			connection[$ "access_token"] = result[$ "access_token"];
			connection[$ "refresh_token"] = result[$ "refresh_token"];
			connection[$ "access_timer"] = call_later(result[$ "expiration_seconds"] ?? 0, time_source_units_seconds, connection.cleanup, false);
			
			var signal = connection.get_signal();

			// If something went wrong with the connection during login, we should logout.
			// We do this here because if we send a request to the API to logout before we've logged in, we'd get a 401.
			// We also make sure we set the access token before making the request, otherwise it will fail with a 401.
			if (signal.get_aborted())
			{
				user_logout(undefined, connection);
				return;
			}

			var player_client = new PlayerClient({
				signal: connection.get_signal(),
				jsonrpc_error: true,
				bearer: connection[$ "access_token"],
			});
			
			return player_client.get_async();
		}))
		.next(method({connection, _logger}, function(result)
		{
			var player = result.player;
			var snapshot = player_snapshot(player);
			var players = [];
			
			with (obj_player)
			{
				/// @feather ignore once GM1041
				array_push(players, player_snapshot(self));
				
				/// @feather ignore once GM1041
				obj_server.notify(self.connection, "player.create_remote", snapshot);
			}
			
			/// @type {Id.Instance.obj_player}
			var inst = instance_create_layer(player.x, player.y, "Instances", obj_player);
			
			inst.name = player.name;
			inst.connection = connection;

			// Send welcome message to player (accounting for new and returning players).
			var messages = [
				{ content: $"Welcome, {player.name}" },
				{ content: "Walk around with WAS or arrow keys, hit the TAB key to chat!" },
				{ content: "Chat now has [rainbow]effects[/rainbow]! Use /[rainbow]rainbow[/rainbow], /[shake]shake[/shake] and [wobble]/wobble[/wobble]!" },
				{ content: "You can now toggle chat visiblity with [c_red]ctrl + t"},
				{ content: "You can now use emotes! Use /exclaim, [c_red]/love, /what and /..." },
			];
			
			_logger.log(log_type.information, $"'{player.name}' logged in!");

			return {
				player: player,
				players: players,
				refresh_token: connection[$ "refresh_token"],
				messages: messages,
			};
		}));
}
