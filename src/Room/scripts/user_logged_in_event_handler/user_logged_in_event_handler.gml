function user_logged_in_event_handler(event)
{
    /// @type {String}
    /// @description The name of the event that triggers the handler.
    static EventName = "UserLoggedInEvent";

    /// @type {Struct.Logger}
    /// @description The logger.
    static _logger = new Logger(nameof(user_logged_in_event_handler));

    if (!is_struct(event))
    {
        _logger.log(log_type.error, $"{EventName} must be a struct.");
        return;
    }

    var username = event[$ "Username"];
    var access_token = event[$ "AccessToken"];

    if (!is_string(username))
    {
        _logger.log(log_type.error, $"Username field is required.");
        return;
    }

    if (!is_string(access_token))
    {
        _logger.log(log_type.error, $"AccessToken field is required.");
        return;
    }

    if (player_exists(username))
    {
        _logger.log(log_type.warning, $"Player '{username}' already exists in this room.");
        return;
    }

    _logger.log(log_type.debug, $"Adding player to room with naem: '{username}'...");

    var player_client = new PlayerClient({
        bearer: access_token,
    });

    player_client
        .get_async()
        .next(method({_logger}, function(response)
        {
            /// @type {Id.Instance.obj_player}
            var inst = instance_create_layer(response.x, response.y, "Instances", obj_player);

            with (inst)
            {
                identifier = response.id;
                name = response.name;
            }

            _logger.log(log_type.information, $"Player joined room with ID: '{inst.identifier}'");

            return inst;
        }));
}