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

			inst.name = response.name;
			inst.identifier = response.identifier;

			event_publish("player.joined", player_get_snapshot(inst));
			
			_logger.log(log_type.information, $"Player joined with name: '{inst.name}'");
		}));
}
