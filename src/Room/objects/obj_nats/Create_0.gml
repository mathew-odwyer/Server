/// @description Initialize default parameters.

#macro event_publish obj_nats._protocol.publish

#macro event_subscribe obj_nats._protocol.subscribe

/// @type {Struct.Logger}
/// @description The logger.
_logger = new Logger(nameof(obj_nats));

/// @type {Struct.Client}
/// @description THe underlying websocket client.
_client = new Client(network_socket_ws);

/// @type {Struct.NatsClientProtocol}
/// @description THe NATS client protocol.
_protocol = new NatsClientProtocol(_client.send);

if (!_client.connect("ws://localhost", 9222))
{
	_logger.log(log_type.error, "Failed to connect to NATS server.");
	throw "Failed to connect to NATS server";
}

event_subscribe("user.logged_in", user_logged_in);
event_subscribe("user.logged_out", user_logged_out);
