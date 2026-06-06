/// @description Provides a NATS client-side protocol handler for a `Struct.Client`.
/// @param {Function} send The function used to send messages through the socket.
/// @param {Struct} options The NATS connection options.
function NatsClientProtocol(send, options = {}) constructor
{
	/// @type {Struct.Logger}
	/// @description The logger instance for logging protocol events.
	static _logger = new Logger(nameof(NatsClientProtocol));

	/// @type {Function}
	/// @description The function used to send messages to the NATS server.
	_send = send;

	/// @type {Struct}
	/// @description The NATS connection options (verbose, pedantic, name, version, etc.).
	_options = options;
	
	/// @type {String}
	/// @description The NATS protocol line terminator.
	_terminator = "\r\n";
	
	/// @type {Struct}
	/// @description Maps subject names to their subscriber callback functions.
	_command_to_handler_map = {};
	
	/// @description Subscribes to a subject and registers a callback to handle incoming messages.
	/// @param {String} subject The subject to subscribe to.
	/// @param {Function} callback The callback function to invoke when a message arrives on the subject. Callback receives (payload, reply_to).
	subscribe = function(subject, callback)
	{
		_logger.log(log_type.trace, $"Subscribing to: '{subject}'");
		var result = _send($"SUB {subject} {subject}{_terminator}");

		if (!result)
		{
			_logger.log(log_type.error, $"Failed to subscribe to: '{subject}'");
			return;
		}

		_command_to_handler_map[$ subject] = callback;
	}
	
	/// @description Publishes a message to the specified subject.
	/// @param {String} subject The subject to publish to.
	/// @param {Any} data The data to publish (will be JSON stringified).
	publish = function(subject, data)
	{
		_logger.log(log_type.trace, $"Publishing to: '{subject}'");

		var payload = json_stringify(data);
		var size = string_byte_length(payload);
		
		var result = _send($"PUB {subject} {size}{_terminator}{payload}{_terminator}");

		if (!result)
		{
			_logger.log(log_type.error, $"Failed to publish to: '{subject}'");
		}
	}

	/// @description Handles an incoming message from the NATS server.
	/// @param {String} payload The raw payload received from the server.
	handle_message = function(payload)
    {
		var parts = string_split(payload, _terminator, true);

		if (array_length(parts) < 1)
		{
			return;
		}

		var control = string_trim(parts[0]);
		var body = array_length(parts) > 1 ? parts[1] : "";
		var parameters = string_split(control, " ", true);

		if (array_length(parameters) == 0)
		{
			return;
		}

		var command = string_upper(parameters[0]);

		switch (command)
		{
			case "INFO":
				_handle_info_command(parameters);
				break;

			case "PING":
				_handle_ping_command();
				break;

			case "MSG":
				_handle_msg_command(parameters, body);
				break;
		}
    }
	
	/// @description Handles an INFO command from the server.
	_handle_info_command = function(parameters)
	{
		var info = json_parse(parameters[1]);
		
		_logger.log(log_type.debug, $"Connecting to: '{info[$ "server_name"]}'...");
		
		var connect = {
			verbose: _options[$ "verbose"] ?? false,
			pedantic: _options[$ "pedantic"] ?? false,
			name: _options[$ "name"] ?? "GameMaker NATS Client",
			lang: "GML",
			version: _options[$ "version"] ?? GM_version,
		};
		
		_send($"CONNECT {json_stringify(connect)}{_terminator}");
	}
	
	/// @description Handles a PING command from the server by responding with PONG.
	_handle_ping_command = function()
	{
		_send($"PONG{_terminator}");
	}

	/// @description Handles an incoming MSG command by routing to the appropriate subscriber callback.
	/// @param {Array<String>} parameters The MSG command parameters parsed from the server message.
	/// @param {String} body The message body/payload.
	_handle_msg_command = function(parameters, body)
	{
		try
		{
			var subject = parameters[1];
			var reply_to = array_length(parameters) > 3 ? parameters[3] : "";
			var handler = _command_to_handler_map[$ subject];

			if (is_undefined(handler))
			{
				_logger.log(log_type.warning, $"No handler registered for subject: '{subject}'");
				return;
			}
			
			handler(json_parse(body), reply_to);
		}
		catch (ex)
		{
			_logger.log(log_type.error, $"Error handling NATS message: {ex}");
			throw new NatsError($"Error handling NATS message.", ex);
		}
	}
}