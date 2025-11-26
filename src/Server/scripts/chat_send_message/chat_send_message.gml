/// @description Sends a chat message to all connected players.
/// @param {Struct} params The parameters of the chat message.
/// @param {Struct.ClientConnection} connection The client connection that sent the message.
function chat_send_message(params, connection)
{
    /// @type {Struct.Logger}
    /// @description The logger.
    static _logger = new Logger(nameof(chat_send_message));
	
	/// @description Extracts an emote from the specified `message`.
	/// @param {String} message The message to extract an emote from.
	/// @returns {Enum.gui_emote_type|Real} Returns the emote type extracted from the message, or -1.
	static _extract_emote = function(message)
	{
		switch (message)
		{
			case "/love":
			case "/heart":
				return gui_emote_type.heart;

			case "/exclaim":
			case "/oi":
				return gui_emote_type.exclaim;
				
			case "/question":
			case "/what":
				return gui_emote_type.question;
				
			case "/...":
			case "/ellipsis":
				return gui_emote_type.ellipsis;
		}
		
		return -1;
	};

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

	var content = string_lower(string_trim(message));
	var emote = _extract_emote(content);

    var dto = {
        sender: string_lower(player.name),
        content: content,
		emote: emote,
    };

    // Notify all connected players of the new chat message.
    // Including the player who sent the message.
    with (obj_player)
    {
        notify("chat.add_message", dto);
    }
}
