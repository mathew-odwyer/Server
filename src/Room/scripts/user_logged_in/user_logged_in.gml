/// @description Handles the `user.logged_in` notification
/// @param {Struct} user The user data of the user who logged in.
function user_logged_in(user)
{
	// TODO: Import Winterhaven core library from Client
	// TODO: Export GMFoundation and Core from Room into Client
	//		- That should keep everything in sync.
	// TODO: Push changes and merge into 103/107
	// TODO: Review changes in client and push changes
	
	// TODO: If we can't fetch the player, we need to notify the gateway.
	// TODO: If anything goes wrong in here, we need to fail fast and publish a message.
	//	     That way the gateway knows the player couldn't join the room.
	
	static _logger = new Logger(nameof(user_logged_in));
	
	var username = user[$ "username"];
	var access_token = user[$ "access_token"];

	var player = player_get_by_name(username);
	
	if (player != noone)
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
			var snapshot = player_get_snapshot(response);
			
			/// @type {Id.Instance.obj_player}
			var inst = instance_create_layer(response.x, response.y, "Instances", obj_player);

			inst.name = response.name;
			
			event_publish("player.joined", player_get_snapshot(response));
			
			_logger.log(log_type.information, $"Player joined with name: '{inst.name}'");
		}));
}
