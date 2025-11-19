/// @description Sends a chat message to all connected players.
/// @param {Struct} params The parameters of the chat message.
/// @param {Struct.ClientConnection} connection The client connection that sent the message.
function chat_send_message(params, connection)
{
    /// @type {Struct.Logger}
    /// @description The logger.
    static _logger = new Logger(nameof(chat_send_message));

    var player = player_get_by_connection(connection);

    if (is_undefined(player))
    {
        _logger.log(log_type.warning, $"Failed to locate player for connection with socket ID: {connection._socket}");
        return;
    }

    var message = params[$ "message"];

    if (string_empty(message))
    {
        return;
    }

    _logger.log(log_type.information, $"Player '{player.name}' sent chat message: '{message}'");

    var dto = {
        name: string_lower(player.name),
        message: string_lower(string_trim(message)),
    };

    // Notify all connected players of the new chat message.
    // Including the player who sent the message.
    with (obj_player)
    {
        notify("chat.add_message", dto);
    }
}
