/// @description Handles the `user.logged_in` notification
/// @param {Struct} user The user data of the user who logged in.
function user_logged_in(user)
{	
	static _logger = new Logger(nameof(user_logged_in));
	
	var username = user[$ "username"];
	var access_token = user[$ "access_token"];

	if (player_exists(username))
	{
		return;
	}
	
	_logger.log(log_type.information, $"Player joining with name: '{username}'");
	
	var player_client = new PlayerClient({
		bearer: access_token,
	});
	
	player_client
		.get_async()
		.next(method({_logger}, function(response)
		{
			/// @type {Id.Instance.obj_player}
			var inst = instance_create_layer(response.x, response.y, "Instances", obj_player);

			inst.identifier = response.id;
			inst.name = response.name;
		
			var snapshot = player_get_snapshot(inst);

			inst.notify("map.load_map", obj_map_loader.map_data);
			inst.notify("player.create_local", snapshot);

			with (obj_player)
			{
				if (self.identifier == inst.identifier)
				{
					continue;
				}

  				// Notify other players of new player.
				notify("player.create_remote", snapshot);
				
				// Notify new player of other players.
				inst.notify("player.create_remote", player_get_snapshot(self));
			}

			// Send welcome message to player (accounting for new and returning players).
			var messages = [
				{ content: $"Welcome, {inst.name}" },
				{ content: "Walk around with WASD or arrow keys, hit the TAB key to chat!" },
				{ content: "Chat now has [rainbow]effects[/rainbow]! Use /[rainbow]rainbow[/rainbow], /[shake]shake[/shake] and [wobble]/wobble[/wobble]!" },
				{ content: "You can now toggle chat visiblity with [c_red]ctrl + t"},
				{ content: "You can now use emotes! Use [c_red]/exclaim, /love, /what and /..." },
			];

			for (var i = 0; i < array_length(messages); i++)
			{
				inst.notify("chat.add_message", messages[i]);
			}

			_logger.log(log_type.information, $"Player joined with name: '{inst.name}'");
		}));
}
