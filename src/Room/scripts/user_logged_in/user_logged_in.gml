function user_logged_in(user)
{
	// TODO: Support lower snake case for both NATS and API with headers or something else?
	// TODO: If we can't fetch the player, we need to notify the gateway.
	// TODO: If anything goes wrong in here, we need to fail fast and publish a message.
	//	     That way the gateway knows the player couldn't join the room.
	
	static _logger = new Logger(nameof(user_logged_in));
	
	var username = user[$ "Username"];
	var access_token = user[$ "AccessToken"];

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
		.next(function(response)
		{
			var snapshot = player_get_snapshot(response);
			
			var inst = instance_create_layer(response.x, response.y, "Instances", obj_player);
			inst.name = response.name;
			
			event_publish("player.joined", player_get_snapshot(response));
			
			_logger.log(log_type.information, $"Player joined with name: '{inst.name}'");
		})
		.fail(function(error)
		{
			_logger.log(log_type.error, $"Player failed to join: {error}");
		});
}
