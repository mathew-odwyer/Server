// TODO: Unit tests
function user_logged_out_event_handler(event)
{
    /// @type {Struct.Logger}
    /// @description The logger.
    static _logger = new Logger(nameof(user_logged_out_event_handler));
    
    /// @type {Struct}
    /// @description The schema of the event.
    static _schema = {
        UserAccountId: t_string,
        AccessToken: t_string,
    };

    assert_type(event, nameof(event), _schema);

    var identifier = event.UserAccountId;
    var access_token = event.AccessToken;

    var player = player_get_by_identifier(identifier);

    if (player == noone)
    {
        _logger.log(log_type.warning, $"Player with ID '{identifier}' doesn't exist in this room.");
        return;
    }

    _logger.log(log_type.debug, $"Removing player from room with ID: '{identifier}'...");

    var player_client = new PlayerClient({
        x_api_key: api_key,
    });

    var dto = {
        player_id: player.identifier,
        x: player.x,
        y: player.y,
    };

    instance_destroy(player);
    player_client.update_async(dto);

    _logger.log(log_type.information, $"Player removed from room with ID: '{player.identifier}'");
}