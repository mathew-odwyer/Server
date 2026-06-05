function user_logged_out_event_handler(event)
{
    /// @type {String}
    /// @description The name of the event that triggers the handler.
    static EventName = "UserLoggedOutEvent";

    /// @type {Struct.Logger}
    /// @description The logger.
    static _logger = new Logger(nameof(user_logged_out_event_handler));

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

    var player = player_get_by_username(username);

    if (player == noone)
    {
        _logger.log(log_type.warning, $"Player '{username}' doesn't exist in this room.");
        return;
    }

    _logger.log(log_type.debug, $"Removing player from room with naem: '{username}'...");

    var player_client = new PlayerClient({
        x_api_key: api_key,
    });

    var dto = {
        player_id: player.identifier,
        x: player.x,
        y: player.y,
    };

    player_client.update_async(dto);
    instance_destroy(player);

    _logger.log(log_type.information, $"Player removed from room with ID: '{dto.player_id}'");
}