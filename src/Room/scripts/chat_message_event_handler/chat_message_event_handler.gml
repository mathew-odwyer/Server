/// @description Handles a chat message event by routing the message to the appropriate player.
/// @param {Any} event The chat event to be handled.
function chat_message_event_handler(event)
{
    /// @type {Struct.Logger}
    /// @description The logger.
    static _logger = new Logger(nameof(chat_message_event_handler));

    /// @type {Struct}
    /// @description The validation schema.
    static _schema = {
        SenderId: t_string,
        Message: t_string,
        EmoteType: t_gui_emote_type,
    };

    assert_type(event, nameof(event), _schema);

    var player = player_get_by_identifier(event.SenderId);

    if (player == noone)
    {
        _logger.log(log_type.warning, $"Failed to locate player with ID: '{event.SenderId}'");
        return;
    }

    var dto = {
        sender_id: player.identifier,
        message: event.Message,
        emote_type: event.EmoteType,
    };

    _logger.log(log_type.debug, $"'{player.name}' says: {event.Message}");

    with (obj_player)
    {
        notify("chat.add_message", dto);
    }
}