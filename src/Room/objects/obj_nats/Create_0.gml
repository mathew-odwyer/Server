/// @description Initialize default parameters.

/// @type {Struct.Logger}
/// @description The logger.
_logger = new Logger(nameof(obj_nats));

/// @type {Struct.Client}
/// @description THe underlying websocket client.
_client = new Client(network_socket_ws);

/// @type {Struct.NatsClientProtocol}
/// @description THe NATS client protocol.
_protocol = new NatsClientProtocol(_client.send);


if (!_client.connect("ws://nats", 9222))
{
	_logger.log(log_type.error, "Failed to connect to NATS server.");
	throw "Failed to connect to NATS server";
}

_protocol.subscribe("user.logged_in", user_logged_in);
_protocol.subscribe("user.logged_out", user_logged_out);
