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

    /// @description Formats the specified `message`.
    /// @param {String} message The message to format.
    /// @returns {String} The formatted message.
    static _format_message = function(message)
    {
        /// @type {Real}
        /// @description The maximum length of a chat message.
        static _max_length = 80;

        /// @type {Array<String>}
        /// @description The array of allowed effects.
        static _allowed_effects = [
            "shake",
            "wobble",
            "rainbow", 
        ];

        var result = string_lower(string_trim(message));

        // Limit message length to predefined maximum.
        result = string_copy(result, 1, min(string_length(result), _max_length));

        // Store the original result to check if it contains actual content.
        var temp = result;

        // Make sure no effects from Scribble are drawn to the client.
        result = string_replace_all(result, "[", "[[")
		
		// Convert any allowed effects to Scribble.
		for (var i = 0; i < array_length(_allowed_effects); i++)
		{
			var effect_name = _allowed_effects[i];
            
            // Apply the effect to the result.
			result = string_replace_all(result, $"/{effect_name} ", $"[{effect_name}]");

            // Check if the original message only contained effect commands (nothing but effects).
            temp = string_replace_all(temp, $"/{effect_name}", "");
		}
        
        // If after removing all effect commands there's no content left, return empty string.
        if (string_empty(string_trim(temp)))
        {
            return "";
        }

		return result;
    }

    var player = player_get_by_connection(connection);

    if (player == noone)
    {
        _logger.log(log_type.warning, $"Failed to locate player for connection with socket ID: {connection.get_identifier()}");
        return;
    }

    var message = params[$ "message"];

    _logger.log(log_type.information, $"Player '{player.name}' sent chat message: '{message}'");

	var content = _format_message(message);
	var emote = _extract_emote(content);

    // If both content and emote are empty/invalid, do not send the message.
    if (string_empty(content) && emote == -1)
    {
        return;
    }

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
